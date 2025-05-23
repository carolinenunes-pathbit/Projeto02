using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Domain.Models;
using Domain.Contracts.Infrastructure;
using Infrastructure.Messaging;
using Infrastructure.Proxy;

async Task RunAsync(string[] args)
{
    try
    {
        // Configura o host com injeção de dependência
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<IMessageService, RabbitMQMessage>();
                services.AddSingleton<EmailProxy>();
                services.AddLogging(configure => configure.AddConsole());
            })
            .UseConsoleLifetime()
            .Build();

        // Configura o token de cancelamento
        using var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true;
            cts.Cancel();
        };

        // Obtém as instâncias dos serviços
        var messageService = host.Services.GetRequiredService<IMessageService>();
        var mailProxy = host.Services.GetRequiredService<EmailProxy>();

        // Função para processar mensagens recebidas
        async Task ProcessMessageAsync(string message)
        {
            try
            {
                var customerData = JsonConvert.DeserializeObject<Customer>(message);
                if (customerData == null || string.IsNullOrWhiteSpace(customerData.Email))
                {
                    return;
                }

                string mailMessage = $"""
                Olá {customerData.Name?.Trim() ?? "Cliente"},

                O seu cadastro está em análise e em breve você receberá um e-mail com 
                novas atualizações sobre o seu cadastro.

                Atenciosamente,
                Equipe PATHBIT
                """;

                // Envia o e-mail de forma assíncrona
                await mailProxy.SendMailAsync(customerData.Email, mailMessage);
            }
            catch (JsonException ex)
            {
                Error(ex, "Erro ao deserializar a mensagem JSON");
            }
            catch (Exception ex)
            {
                Error(ex, "Erro ao processar a mensagem ou enviar o e-mail");
            }
        }

        try 
        {
            // Inicia o host em uma tarefa separada
            var hostTask = host.StartAsync(cts.Token);
            
            // Registra o consumidor
            await messageService.GetMessageAsync(
                queue: "cadastro.em.analise.email",
                callbackMethod: msg => ProcessMessageAsync(msg).GetAwaiter().GetResult()
            );
            
            // Mantém o serviço rodando até que seja cancelado
            await host.WaitForShutdownAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Serviço sendo encerrado...");
        }
        finally
        {
            // Garante que o host seja encerrado corretamente
            if (host is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync();
            }
            else
            {
                (host as IDisposable)?.Dispose();
            }
        }
    }
    catch (Exception ex)
    {
        Error(ex, "Falha crítica ao iniciar o serviço");
        Environment.Exit(1);
    }
}

// Executa a aplicação
await RunAsync(args);