using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Domain.Contracts.Infrastructure;
using System.Text;

namespace Infrastructure.Messaging;

public class RabbitMqMessage : IMessageService, IDisposable
{
    private IConnection _connection;
    private IChannel _channel;
    private readonly ConnectionFactory _factory;
    private const string ROUTING_KEY = "cadastro.em.analise.email";

    public RabbitMqMessage()
    {
        _factory = new ConnectionFactory { HostName = "rabbitmq" };
        Console.WriteLine($"[LOG] {DateTime.Now}: ConnectionFactory criada com HostName: {_factory.HostName}");
    }

    private async ValueTask ConnectAsync()
    {
        Console.WriteLine($"[LOG] {DateTime.Now}: Tentando conectar ao RabbitMQ...");
        if (_connection is null || !_connection.IsOpen)
        {
            try
            {
                _connection = await _factory.CreateConnectionAsync();
                Console.WriteLine($"[LOG] {DateTime.Now}: Conexão estabelecida com sucesso.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERRO] {DateTime.Now}: Erro ao conectar: {ex.Message}");
                throw; // Re-lança a exceção para ser tratada no SendMessageAsync
            }
        }

        if (_channel is null || !_channel.IsOpen)
        {
            try
            {
                _channel = await _connection.CreateChannelAsync();
                Console.WriteLine($"[LOG] {DateTime.Now}: Canal criado com sucesso.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERRO] {DateTime.Now}: Erro ao criar canal: {ex.Message}");
                throw;
            }
        }
    }

    public async ValueTask SendMessageAsync(string queue, string message)
    {
        Console.WriteLine($"[LOG] {DateTime.Now}: Tentando enviar mensagem: '{message}' para a fila: '{queue}'");
        try
        {
            await ConnectAsync();
            Console.WriteLine($"[LOG] {DateTime.Now}: Conexão e canal verificados/criados.");

            Console.WriteLine($"[LOG] {DateTime.Now}: Declarando fila: '{queue}'...");
            await _channel.QueueDeclareAsync(queue: queue,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            Console.WriteLine($"[LOG] {DateTime.Now}: Fila '{queue}' declarada com sucesso.");

            var body = Encoding.UTF8.GetBytes(message);
            Console.WriteLine($"[LOG] {DateTime.Now}: Mensagem convertida para bytes.");

            Console.WriteLine($"[LOG] {DateTime.Now}: Publicando mensagem para a fila: '{queue}' com Routing Key: '{ROUTING_KEY}'...");
            await IChannelExtensions.BasicPublishAsync(channel: _channel,
                exchange: string.Empty,
                routingKey: ROUTING_KEY,
                body: body);
            Console.WriteLine($"[LOG] {DateTime.Now}: Mensagem publicada com sucesso para a fila: '{queue}'.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERRO] {DateTime.Now}: Erro ao enviar mensagem para a fila '{queue}': {ex.Message}");
        }
    }

    public async Task GetMessageAsync(string queue, Action<string> callbackMethod)
    {
        Console.WriteLine($"[LOG] {DateTime.Now}: Tentando receber mensagens da fila: '{queue}'...");
        try
        {
            await ConnectAsync();
            Console.WriteLine($"[LOG] {DateTime.Now}: Conexão e canal verificados/criados para receber.");

            Console.WriteLine($"[LOG] {DateTime.Now}: Declarando fila: '{queue}' para consumo...");
            await _channel.QueueDeclareAsync(queue: queue,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            Console.WriteLine($"[LOG] {DateTime.Now}: Fila '{queue}' declarada com sucesso para consumo.");

            Console.WriteLine($"[LOG] {DateTime.Now}: Criando consumidor para a fila: '{queue}'...");
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                Console.WriteLine($"[LOG] {DateTime.Now}: Mensagem recebida.");
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                callbackMethod(message);
                await Task.Yield(); // Para evitar warnings de método async sem await no caminho principal
            };
            Console.WriteLine($"[LOG] {DateTime.Now}: Consumidor criado.");

            Console.WriteLine($"[LOG] {DateTime.Now}: Iniciando consumo da fila: '{queue}'...");
            await _channel.BasicConsumeAsync(queue: queue, // Usando o nome da fila
                                               autoAck: true,
                                               consumer: consumer);
            Console.WriteLine($"[LOG] {DateTime.Now}: Consumo iniciado na fila: '{queue}'.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERRO] {DateTime.Now}: Erro ao configurar o consumo da fila '{queue}': {ex.Message}");
        }
    }

    public void Dispose()
    {
        if (_channel != null && _channel.IsOpen)
            _channel.Dispose();
        if (_connection != null && _connection.IsOpen)
            _connection.Dispose();
        Console.WriteLine($"[LOG] {DateTime.Now}: Conexão e canal Disposed.");
	}
}