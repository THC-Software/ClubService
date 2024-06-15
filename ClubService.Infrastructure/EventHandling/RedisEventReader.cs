using System.Text.Json.Nodes;
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
    private readonly ILoggerService<RedisEventReader> _loggerService;
    private readonly int _pollingInterval;
    private readonly List<RedisStream> _redisStreams;
    private readonly IServiceProvider _services;

    public RedisEventReader(
        IServiceProvider services,
        IOptions<RedisConfiguration> redisConfig,
        ILoggerService<RedisEventReader> loggerService)
    {
        _pollingInterval = redisConfig.Value.PollingInterval;
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
        try
        {
            foreach (var stream in _redisStreams)
            {
                var result = await db.StreamReadGroupAsync(stream.StreamName, stream.ConsumerGroup,
                    "pos-member", ">", 1);

                if (result.Length == 0)
                {
                    continue;
                }

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

                await db.StreamAcknowledgeAsync(stream.StreamName, stream.ConsumerGroup, streamEntry.Id);
            }
        }
        catch (InvalidOperationException e)
        {
            _loggerService.LogInvalidOperationException(e);
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

        using PeriodicTimer timer = new(TimeSpan.FromSeconds(_pollingInterval));

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
    }
}