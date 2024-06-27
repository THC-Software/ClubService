using ClubService.Infrastructure.Configurations;
using ClubService.Infrastructure.EventHandling;
using ClubService.Infrastructure.Mail;

namespace ClubService.API.ApplicationConfigurations;

public static class ExternalServiceExtensions
{
    public static void AddExternalServiceConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SmtpConfiguration>(configuration.GetSection("SmtpConfiguration"));
        services.AddHostedService<EmailMessageRelay>();

        services.Configure<RedisConfiguration>(configuration.GetSection("RedisConfiguration"));
        services.AddHostedService<RedisEventReader>();
    }
}