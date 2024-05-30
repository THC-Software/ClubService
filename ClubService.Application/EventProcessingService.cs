using ClubService.Infrastructure.Api;
using Microsoft.Extensions.Hosting;

namespace ClubService.Application;

public class EventProcessingService(IEventReader eventReader) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await eventReader.ConsumeMessagesAsync();
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Redis Event Reader stopped!");
        }
    }
}