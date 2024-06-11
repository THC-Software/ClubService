using ClubService.Domain.Repository;
using Microsoft.Extensions.Logging;

namespace ClubService.Infrastructure.Logging;

public class LoggerService<T>(ILogger<T> logger) : ILoggerService<T>
{
    public void LogInformation(string message)
    {
        logger.LogInformation(message);
    }

    public void LogWarning(string message)
    {
        logger.LogWarning(message);
    }

    public void LogError(string message, Exception exception)
    {
        logger.LogError(exception, message);
    }

    public void LogDebug(string message)
    {
        logger.LogDebug(message);
    }
}