using System.Net.Mail;
using ClubService.Domain.Repository;
using ClubService.Infrastructure.Configurations;
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
    private readonly IServiceProvider _services = services;
    private readonly SmtpClient _smtpClient = new(smtpConfiguration.Value.Host, smtpConfiguration.Value.Port);

    private void SendEmails()
    {
        throw new NotImplementedException();
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
        }
        catch (Exception e)
        {
            loggerService.LogException(e);
        }
    }
}