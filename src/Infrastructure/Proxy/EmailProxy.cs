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
        Console.WriteLine($"[LOG] {DateTime.Now}: Método SendMailAsync chamado para o email: {email}");

        Env.Load();
        var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
        var client = new SendGridClient(apiKey);
        var from_email = new EmailAddress("caroline.nunes.pathbit@gmail.com", "Caroline");
        var subject = "Cadastro de Clientes";
        var to_email = new EmailAddress(email);
        var plainTextContent = message;
        var htmlContent = message;
        var msg = MailHelper.CreateSingleEmail(from_email, to_email, subject, plainTextContent, htmlContent);
        var response = await client.SendEmailAsync(msg).ConfigureAwait(false);

        Console.WriteLine($"[LOG] {DateTime.Now}: Resposta do SendGrid: StatusCode = {response.StatusCode}, IsSuccessStatusCode = {response.IsSuccessStatusCode}");
	}
}