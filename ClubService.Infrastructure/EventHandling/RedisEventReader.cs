using System.Text.Json;
using ClubService.Application.EventHandlers;
using ClubService.Domain.Repository;
using ClubService.Infrastructure.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using RedisStream = ClubService.Infrastructure.Configurations.RedisStream;

namespace ClubService.Infrastructure.EventHandling;

public class RedisEventReader : BackgroundService
{
    private const string AllEvents = "*";
    private readonly ILoggerService<RedisEventReader> _loggerService;
    private readonly int _pollingInterval;
    private readonly List<RedisStream> _redisStreams;
    private readonly IServiceProvider _services;

    public RedisEventReader(
        IServiceProvider services,
        IOptions<RedisConfiguration> redisConfig,
        ILoggerService<RedisEventReader> loggerService)
    {
        _pollingInterval = redisConfig.Value.PollingIntervalMilliSeconds;
        _services = services;
        _loggerService = loggerService;
        _redisStreams = redisConfig.Value.Streams;
        var configurationOptions = ConfigurationOptions.Parse(redisConfig.Value.Host);
        configurationOptions.AbortOnConnectFail = false; // Allow retrying
        connectionMultiplexer = ConnectionMultiplexer.Connect(configurationOptions);
        db = connectionMultiplexer.GetDatabase();
    }

    private IDatabase db { get; }
    private ConnectionMultiplexer connectionMultiplexer { get; }

    private async Task ConsumeMessages()
    {
        foreach (var stream in _redisStreams)
        {
            var entries = await db.StreamReadGroupAsync(stream.StreamName, stream.ConsumerGroup,
                "club-service", ">", 1);

            foreach (var entry in entries)
            {
                var jsonValue = entry.Values.FirstOrDefault().Value.ToString();

                if (string.IsNullOrWhiteSpace(jsonValue))
                {
                    _loggerService.LogEmptyStreamEntry();
                    continue;
                }

                JsonDocument document;
                try
                {
                    document = JsonDocument.Parse(jsonValue);
                }
                catch (JsonException ex)
                {
                    _loggerService.LogJsonException(ex, jsonValue);
                    await db.StreamAcknowledgeAsync(stream.StreamName, stream.ConsumerGroup, entry.Id);
                    continue;
                }

                if (!document.RootElement.TryGetProperty("payload", out var payload) ||
                    !payload.TryGetProperty("after", out var after) ||
                    after.ValueKind == JsonValueKind.Null)
                {
                    _loggerService.LogJsonMissingProperties(jsonValue);
                    await db.StreamAcknowledgeAsync(stream.StreamName, stream.ConsumerGroup, entry.Id);
                    continue;
                }
                
                // Here we filter out the events we don't want.
                // If the EventTypes list contains an asterisk we want all the events of that stream.
                // Otherwise, we check if the event type of the parsedEvent is in the list of desired events.
                var parsedEventType = after.GetProperty("eventType").GetString();
                if (!stream.DesiredEventTypes.Contains(AllEvents) &&
                    !stream.DesiredEventTypes.Contains(parsedEventType, StringComparer.OrdinalIgnoreCase))
                {
                    await db.StreamAcknowledgeAsync(stream.StreamName, stream.ConsumerGroup, entry.Id);
                    continue;
                }
                
                var parsedEvent = EventParser.ParseEvent(after);

                using var scope = _services.CreateScope();
                var chainEventHandler = scope.ServiceProvider.GetRequiredService<ChainEventHandler>();
                await chainEventHandler.Handle(parsedEvent);

                await db.StreamAcknowledgeAsync(stream.StreamName, stream.ConsumerGroup, entry.Id);
            }
        }
    }

    private async Task EnsureStreamAndGroupExists(RedisStream stream)
    {
        if (!await db.KeyExistsAsync(stream.StreamName) ||
            (await db.StreamGroupInfoAsync(stream.StreamName)).All(x => x.Name != stream.ConsumerGroup))
        {
            await db.StreamCreateConsumerGroupAsync(stream.StreamName, stream.ConsumerGroup, "0-0");
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _loggerService.LogEventReaderStart();

        foreach (var redisStream in _redisStreams)
        {
            await EnsureStreamAndGroupExists(redisStream);
        }

        using PeriodicTimer timer = new(TimeSpan.FromMilliseconds(_pollingInterval));

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await ConsumeMessages();
            }
        }
        catch (OperationCanceledException)
        {
            await connectionMultiplexer.DisposeAsync();
            _loggerService.LogEventReaderStop();
        }
        catch (Exception e)
        {
            _loggerService.LogException(e);
        }
    }
}