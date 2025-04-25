using Infrastructure.Messaging;
using Infrastructure.Proxy;
using Newtonsoft.Json;
using Domain.Models;

Console.WriteLine($"[LOG] {DateTime.Now}: Email Service Iniciando...");

var messageService = new RabbitMqMessage();
var mailProxy = new EmailProxy();

void ProcessMessage(string message)
{
    Console.WriteLine($"[LOG] {DateTime.Now}: Mensagem recebida para processamento: {message}");
    try
    {
        var customerData = JsonConvert.DeserializeObject<Customer>(message);
        string mailMessage = string.Format(
            $"Olá {customerData.Name},\n" +
            "O seu cadastro está em análise e em breve você receberá um e-mail com novas atualizações sobre seu cadastro.\n" +
            "Atenciosamente,\n" +
            "Equipe PATHBIT"
        );
        Console.WriteLine($"[LOG] {DateTime.Now}: Enviando e-mail para: {customerData.Email}");
        mailProxy.SendMailAsync(customerData.Email, mailMessage);
        Console.WriteLine($"[LOG] {DateTime.Now}: E-mail enviado para: {customerData.Email}");
    }
    catch (JsonException ex)
    {
        Console.WriteLine($"[ERRO] {DateTime.Now}: Erro ao deserializar a mensagem JSON: {ex.Message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ERRO] {DateTime.Now}: Erro ao processar a mensagem ou enviar o e-mail: {ex.Message}");
    }
}

Console.WriteLine($"[LOG] {DateTime.Now}: Chamando GetMessageAsync...");
await messageService.GetMessageAsync(
    "cadastro.em.analise.email",
    ProcessMessage
);
Console.WriteLine($"[LOG] {DateTime.Now}: GetMessageAsync retornou.");

Console.WriteLine("[LOG] {DateTime.Now}: Recebendo Mensagens ...");
Console.ReadLine();

Console.WriteLine($"[LOG] {DateTime.Now}: Email Service Finalizado.");