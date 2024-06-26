using System.Net.Mail;
using ClubService.Domain.Repository;
using ClubService.Domain.Repository.Transaction;
using ClubService.Infrastructure.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ClubService.Infrastructure.Mail;

public class EmailMessageRelay : BackgroundService
{
    private readonly ILoggerService<EmailMessageRelay> _loggerService;
    private readonly int _pollingInterval;
    private readonly string _senderEmailAddress;
    private readonly IServiceProvider _serviceProvider;
    private readonly SmtpClient _smtpClient;

    public EmailMessageRelay(
        IServiceProvider serviceProvider,
        IOptions<SmtpConfiguration> smtpConfiguration,
        ILoggerService<EmailMessageRelay> loggerService)
    {
        _serviceProvider = serviceProvider;
        _loggerService = loggerService;
        _pollingInterval = smtpConfiguration.Value.PollingInterval;
        _senderEmailAddress = smtpConfiguration.Value.SenderEmailAddress;
        _smtpClient = new SmtpClient(smtpConfiguration.Value.Host, smtpConfiguration.Value.Port);
        _smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
    }

    private async Task SendEmails()
    {
        using var scope = _serviceProvider.CreateScope();
        var emailOutboxRepository = scope.ServiceProvider.GetRequiredService<IEmailOutboxRepository>();
        var readStoreTransactionManager = scope.ServiceProvider.GetRequiredService<IReadStoreTransactionManager>();

        await readStoreTransactionManager.TransactionScope(async () =>
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
                await SendEmails();
            }
        }
        catch (OperationCanceledException)
        {
            _loggerService.LogEmailMessageRelayStop();
            _smtpClient.Dispose();
        }
        catch (Exception e)
        {
            _loggerService.LogException(e);
        }
    }
}