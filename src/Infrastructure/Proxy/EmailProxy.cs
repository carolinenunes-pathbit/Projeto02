using Domain.Contracts.Infrastructure;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;
using DotNetEnv;

namespace Infrastructure.Proxy;

public class EmailProxy: IMailProxy
{
    public async ValueTask SendMailAsync(string email, string message)
{
    if (string.IsNullOrWhiteSpace(email))
        throw new ArgumentException("Email não pode ser vazio", nameof(email));
    
    if (string.IsNullOrWhiteSpace(message))
        throw new ArgumentException("Mensagem não pode ser vazia", nameof(message));

    try
    {
        var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
        if (string.IsNullOrEmpty(apiKey))
            throw new InvalidOperationException("Chave da API do SendGrid não configurada");

        var client = new SendGridClient(apiKey);
        var from = new EmailAddress("caroline.luiza.pathbit@gmail.com", "Caroline");
        var to = new EmailAddress(email);
        var subject = "Cadastro de Clientes";
        var plainTextContent = message;
        var htmlContent = $"<p>{message}</p>";
        
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        var response = await client.SendEmailAsync(msg).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Body.ReadAsStringAsync();
            throw new Exception($"Falha ao enviar email. Status: {response.StatusCode}, Detalhes: {errorContent}");
        }

    }
    catch (Exception ex)
    {
        throw;
    }
}
}