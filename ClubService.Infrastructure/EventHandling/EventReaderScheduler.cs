﻿using ClubService.Application.Api;
using Microsoft.Extensions.Hosting;

namespace ClubService.Infrastructure.EventHandling;

public class EventReaderScheduler(IEventReader eventReader) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await eventReader.ConsumeMessagesAsync(stoppingToken);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("BackgroundService Stopped!");
            Dispose();
        }
    }

    public override void Dispose()
    {
        eventReader.Dispose();
        base.Dispose();
    }
}