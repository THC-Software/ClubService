using System.Net.Mail;
using ClubService.Domain.Repository;
using ClubService.Domain.Repository.Transaction;
using ClubService.Infrastructure.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ClubService.Infrastructure.Mail;

public class EmailMessageRelay(
    IServiceProvider services,
    IOptions<SmtpConfiguration> smtpConfiguration,
    ILoggerService<EmailMessageRelay> loggerService)
    : BackgroundService
{
    private readonly int _pollingInterval = smtpConfiguration.Value.PollingInterval;
    private readonly string _senderEmailAddress = smtpConfiguration.Value.SenderEmailAddress;
    private readonly SmtpClient _smtpClient = new(smtpConfiguration.Value.Host, smtpConfiguration.Value.Port);

    private void SendEmails()
    {
        using var scope = services.CreateScope();
        var emailOutboxRepository = scope.ServiceProvider.GetRequiredService<IEmailOutboxRepository>();
        var readStoreTransactionManager = scope.ServiceProvider.GetRequiredService<IReadStoreTransactionManager>();
        _smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

        readStoreTransactionManager.TransactionScope(async () =>
        {
            var emails = await emailOutboxRepository.GetAllEmails();

            foreach (var email in emails)
            {
                var mailMessage = new MailMessage(_senderEmailAddress, email.RecipientEMailAddress)
                {
                    Subject = email.Subject,
                    Body = email.Body
                };

                await _smtpClient.SendMailAsync(mailMessage);
            }

            await emailOutboxRepository.RemoveEmails(emails);
        });
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new(TimeSpan.FromSeconds(_pollingInterval));

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                SendEmails();
            }
        }
        catch (OperationCanceledException)
        {
            loggerService.LogEmailMessageRelayStop();
            _smtpClient.Dispose();
        }
        catch (Exception e)
        {
            loggerService.LogException(e);
        }
    }
}