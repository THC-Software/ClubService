using System.Net.Mail;
using ClubService.Domain.Repository;
using ClubService.Infrastructure.Configurations;
using Microsoft.Extensions.Options;

namespace ClubService.Infrastructure.Mail;

public class MailService(IOptions<SmtpConfiguration> smtpConfig) : IMailService
{
    private readonly string _host = smtpConfig.Value.Host;
    private readonly int _port = smtpConfig.Value.Port;

    public async Task Send(string email, string subject, string body)
    {
        var mailMessage = new MailMessage("admin@thcdornbirn.at", email)
        {
            Subject = subject,
            Body = body
        };

        using var smtpClient = new SmtpClient(_host, _port);
        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
        await smtpClient.SendMailAsync(mailMessage);
    }
}