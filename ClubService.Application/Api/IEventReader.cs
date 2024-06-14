namespace ClubService.Application.Api;

public interface IEventReader : IDisposable
{
    Task ConsumeMessagesAsync(CancellationToken cancellationToken);
}