namespace ClubService.Infrastructure.Api;

public interface IEventReader : IDisposable
{
    Task ConsumeMessagesAsync();
}