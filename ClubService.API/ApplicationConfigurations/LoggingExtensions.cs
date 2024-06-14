using ClubService.Domain.Repository;
using ClubService.Infrastructure.Logging;

namespace ClubService.API.ApplicationConfigurations;

public static class LoggingExtensions
{
    public static void AddLoggingConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddLogging(config =>
        {
            config.AddConsole();
            config.AddDebug();
            config.AddConfiguration(configuration.GetSection("Logging"));
        });
        services.AddTransient(typeof(ILoggerService<>), typeof(LoggerService<>));
    }
}