namespace ClubService.Domain.Repository;

public interface ILoggerService<T>
{
    void LogInformation(string message);
    void LogWarning(string message);
    void LogError(string message, Exception exception);
    void LogDebug(string message);
}