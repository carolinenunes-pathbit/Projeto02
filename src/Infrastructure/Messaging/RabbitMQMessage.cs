using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Domain.Contracts.Infrastructure;

// Explicitly reference the RabbitMQ.Client assembly to ensure IModel is found
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(RabbitMQ.Client.IModel))]

namespace Infrastructure.Messaging;

public class RabbitMQMessage : IMessageService, IDisposable
{
    private IConnection? _connection;
    private IModel? _channel;
    private readonly ConnectionFactory _factory;
    private const string ROUTING_KEY = "cadastro.em.analise.email";
    private readonly object _lock = new();
    private bool _disposed;
    private const int MAX_RETRY_ATTEMPTS = 5;
    private const int RETRY_DELAY_MS = 5000;

    public RabbitMQMessage()
    {
        _factory = new ConnectionFactory
        {
            HostName = "rabbitmq",
            Port = 5672,
            UserName = "guest",
            Password = "guest",
            VirtualHost = "/",
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
        };
    }

    private async Task EnsureConnected()
    {
        if (_connection?.IsOpen == true && _channel?.IsOpen == true)
        {
            return;
        }

        int retryCount = 0;
        Exception? lastError = null;

        while (retryCount < MAX_RETRY_ATTEMPTS)
        {
            try
            {
                // Fecha conexões existentes de forma assíncrona
                if (_channel != null)
                {
                    if (_channel.IsOpen) await Task.Run(() => _channel.Close());
                    _channel.Dispose();
                    _channel = null;
                }

                if (_connection != null)
                {
                    if (_connection.IsOpen) await Task.Run(() => _connection.Close());
                    _connection.Dispose();
                    _connection = null;
                }


                // Cria nova conexão de forma assíncrona
                _connection = await Task.Run(() => _factory.CreateConnection());
                _connection.ConnectionShutdown += OnConnectionShutdown;
                _connection.CallbackException += OnCallbackException;
                _connection.ConnectionBlocked += OnConnectionBlocked;
                
                _channel = await Task.Run(() => _connection.CreateModel());
                return;
            }
            catch (Exception ex) when (retryCount < MAX_RETRY_ATTEMPTS - 1)
            {
                lastError = ex;
                retryCount++;
                await Task.Delay(RETRY_DELAY_MS);
            }
            catch (Exception ex)
            {
                lastError = ex;
                break;
            }
        }

        throw new InvalidOperationException($"Não foi possível conectar ao RabbitMQ após {MAX_RETRY_ATTEMPTS} tentativas", lastError);
    }

    private void OnConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        Console.WriteLine($"Conexão com RabbitMQ encerrada. Motivo: {e.ReplyText}");
    }

    private void OnCallbackException(object? sender, CallbackExceptionEventArgs e)
    {
        Console.WriteLine($"Erro no callback do RabbitMQ: {e.Exception}");
    }

    private void OnConnectionBlocked(object? sender, ConnectionBlockedEventArgs e)
    {
        Console.WriteLine($"Conexão com RabbitMQ bloqueada. Motivo: {e.Reason}");
    }

    public async ValueTask SendMessageAsync(string queue, string message)
    {
        if (string.IsNullOrWhiteSpace(queue))
            throw new ArgumentException("O nome da fila não pode ser vazio", nameof(queue));
            
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("A mensagem não pode ser vazia", nameof(message));

        try
        {
            await EnsureConnected();
            if (_channel == null)
                throw new InvalidOperationException("Canal não inicializado");

            var body = Encoding.UTF8.GetBytes(message);
            
            _channel.BasicPublish(
                exchange: string.Empty,
                routingKey: queue,
                mandatory: true,
                basicProperties: null,
                body: body);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Falha ao enviar mensagem para a fila '{queue}': {ex}");
            throw;
        }
    }

    public async Task GetMessageAsync(string queue, Action<string> callbackMethod)
    {
        if (string.IsNullOrWhiteSpace(queue))
            throw new ArgumentException("O nome da fila não pode ser vazio", nameof(queue));
            
        if (callbackMethod == null)
            throw new ArgumentNullException(nameof(callbackMethod));

        await EnsureConnected();
        if (_channel == null)
            throw new InvalidOperationException("Canal não inicializado");

        try
        {
            // Declara a fila se não existir
            _channel.QueueDeclare(
                queue: queue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            
            // Configura o QoS para processar uma mensagem por vez
            _channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(_channel);
            
            consumer.Received += (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    
                    // Processa a mensagem
                    callbackMethod(message);
                    
                    // Confirma o processamento
                    _channel?.BasicAck(ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    // Nega o reconhecimento da mensagem para que ela seja reprocessada
                    try
                    {
                        _channel?.BasicNack(ea.DeliveryTag, multiple: false, requeue: true);
                    }
                    catch (Exception nackEx)
                    {
                        Console.WriteLine($"Falha ao negar reconhecimento da mensagem: {nackEx}");
                    }
                }
            };

            // Inicia o consumo de mensagens
            var consumerTag = _channel.BasicConsume(
                queue: queue,
                autoAck: false,
                consumer: consumer);
                
            // Retorna uma tarefa que nunca completa, mantendo o consumidor ativo
            await Task.Delay(Timeout.Infinite);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Falha ao configurar o consumo da fila '{queue}': {ex}");
            throw;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~RabbitMQMessage()
    {
        Dispose(false);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        
        if (disposing)
        {
            try
            {
                // Libera os recursos gerenciados
                if (_channel != null)
                {
                    if (_channel.IsOpen)
                        _channel.Close();
                    _channel.Dispose();
                }
                
                if (_connection != null)
                {
                    if (_connection.IsOpen)
                        _connection.Close();
                    _connection.Dispose();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao liberar recursos: {ex.Message}");
            }
            finally
            {
                _channel = null;
                _connection = null;
                _disposed = true;
            }
        }
    }
}