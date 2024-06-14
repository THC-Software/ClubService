using System.Text.Json.Nodes;
using ClubService.Application.EventHandlers;
using ClubService.Infrastructure.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace ClubService.Infrastructure.EventHandling;

public class RedisEventReader : BackgroundService
{
    private readonly string _groupName;
    private readonly IServiceProvider _services;
    private readonly string _streamName;
    private IDatabase db { get; }
    private ConnectionMultiplexer connectionMultiplexer { get; }

    public RedisEventReader(IServiceProvider services, IOptions<RedisConfiguration> redisConfig)
    {
        _streamName = redisConfig.Value.StreamName;
        _groupName = redisConfig.Value.ConsumerGroup;
        _services = services;

        var configurationOptions = ConfigurationOptions.Parse(redisConfig.Value.Host);
        configurationOptions.AbortOnConnectFail = false; // Allow retrying
        connectionMultiplexer = ConnectionMultiplexer.Connect(configurationOptions);
        db = connectionMultiplexer.GetDatabase();
    }

    private async Task ConsumeMessagesAsync()
    {
        if (!await db.KeyExistsAsync(_streamName) ||
            (await db.StreamGroupInfoAsync(_streamName)).All(x => x.Name != _groupName))
        {
            await db.StreamCreateConsumerGroupAsync(_streamName, _groupName, "0-0");
        }

        var id = string.Empty;
        try
        {
            if (!string.IsNullOrEmpty(id))
            {
                await db.StreamAcknowledgeAsync(_streamName, _groupName, id);
            }

            var result = await db.StreamReadGroupAsync(_streamName, _groupName, "pos-member", ">", 1);
            if (result.Any())
            {
                var streamEntry = result.First();
                var dict = streamEntry.Values.ToDictionary(x => x.Name.ToString(), x => x.Value.ToString());
                var jsonContent = JsonNode.Parse(dict.Values.First());

                if (jsonContent == null)
                {
                    throw new InvalidOperationException("json content is null");
                }

                var payload = jsonContent["payload"];
                if (payload == null)
                {
                    throw new InvalidOperationException("payload is null");
                }

                var eventInfo = payload["after"];
                if (eventInfo == null)
                {
                    throw new InvalidOperationException("event info is null");
                }

                var parsedEvent = EventParser.ParseEvent(eventInfo);

                using var scope = _services.CreateScope();
                var chainEventHandler = scope.ServiceProvider.GetRequiredService<ChainEventHandler>();
                await chainEventHandler.Handle(parsedEvent);
            }
        }
        catch (InvalidOperationException e)
        {
            //TODO: Use logger
            Console.WriteLine("Event Ignored: " + e.Message);
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await ConsumeMessagesAsync();

        using PeriodicTimer timer = new(TimeSpan.FromSeconds(1));

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await ConsumeMessagesAsync();
            }
        }
        catch (OperationCanceledException)
        {
            await connectionMultiplexer.DisposeAsync();
            Console.WriteLine("Timed Hosted Service is stopping.");
        }
    }
}