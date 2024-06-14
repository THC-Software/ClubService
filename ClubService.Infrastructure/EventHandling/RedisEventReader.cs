using System.Text.Json.Nodes;
using ClubService.Application.Api;
using ClubService.Application.EventHandlers;
using ClubService.Infrastructure.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace ClubService.Infrastructure.EventHandling;

public class RedisEventReader : IEventReader
{
    private readonly CancellationToken _cancellationToken;
    private readonly IDatabase _db;
    private readonly string _groupName;
    private readonly ConnectionMultiplexer _muxer;
    private readonly IServiceProvider _services;
    private readonly string _streamName;

    public RedisEventReader(
        IServiceProvider services,
        IOptions<RedisConfiguration> redisConfig,
        CancellationToken cancellationToken)
    {
        _streamName = redisConfig.Value.StreamName;
        _groupName = redisConfig.Value.ConsumerGroup;
        _services = services;
        _cancellationToken = cancellationToken;
        var configurationOptions = ConfigurationOptions.Parse(redisConfig.Value.Host);
        configurationOptions.AbortOnConnectFail = false; // Allow retrying
        _muxer = ConnectionMultiplexer.Connect(configurationOptions);
        _db = _muxer.GetDatabase();
    }

    public async Task ConsumeMessagesAsync()
    {
        if (!await _db.KeyExistsAsync(_streamName) ||
            (await _db.StreamGroupInfoAsync(_streamName)).All(x => x.Name != _groupName))
        {
            await _db.StreamCreateConsumerGroupAsync(_streamName, _groupName, "0-0");
        }

        var id = string.Empty;
        while (!_cancellationToken.IsCancellationRequested)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    await _db.StreamAcknowledgeAsync(_streamName, _groupName, id);
                    id = string.Empty;
                }

                var result = await _db.StreamReadGroupAsync(_streamName, _groupName, "pos-member", ">", 1);
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

                await Task.Delay(1000, _cancellationToken);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Redis Event Reader stopped!");
                Dispose();
                break;
            }
            catch (InvalidOperationException e)
            {
                //TODO: Use logger
                Console.WriteLine("Event Ignored: " + e.Message);
            }
        }
    }

    public void Dispose()
    {
        _muxer.Dispose();
    }
}