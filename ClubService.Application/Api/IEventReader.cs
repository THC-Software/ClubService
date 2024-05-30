namespace ClubService.Infrastructure.Api;

public interface IEventReader
{
    Task ConsumeMessagesAsync();
}