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
    private readonly MailAddress _senderEmailAddress;
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
        _senderEmailAddress = new MailAddress(smtpConfiguration.Value.SenderEmailAddress);
        _smtpClient = new SmtpClient(smtpConfiguration.Value.Host, smtpConfiguration.Value.Port);
        _smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
    }

    private async Task SendEmails()
    {
        using var scope = _serviceProvider.CreateScope();
        var emailOutboxRepository = scope.ServiceProvider.GetRequiredService<IEmailOutboxRepository>();
        var transactionManager = scope.ServiceProvider.GetRequiredService<ITransactionManager>();

        var emailMessages = await emailOutboxRepository.GetAllEmailMessages();

        foreach (var emailMessage in emailMessages)
        {
            await transactionManager.TransactionScope(async () =>
            {
                MailAddress recipientEmailAddress;
                try
                {
                    recipientEmailAddress = new MailAddress(emailMessage.RecipientEMailAddress);
                }
                catch (FormatException)
                {
                    _loggerService.LogInvalidEMailAddress(emailMessage.RecipientEMailAddress);
                    throw;
                }

                var mailMessage = new MailMessage(_senderEmailAddress, recipientEmailAddress)
                {
                    Subject = emailMessage.Subject,
                    Body = emailMessage.Body
                };

                await _smtpClient.SendMailAsync(mailMessage);
                await emailOutboxRepository.Delete(emailMessage);
            });
        }
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