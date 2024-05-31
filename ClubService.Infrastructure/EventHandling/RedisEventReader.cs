using System.Text.Json.Nodes;
using ClubService.Application.Api;
using ClubService.Domain.Event;
using ClubService.Infrastructure.Api;
using StackExchange.Redis;

namespace ClubService.Infrastructure.EventHandling;

public class RedisEventReader : IEventReader
{
    private readonly CancellationToken _cancellationToken;
    private readonly string _streamName;
    private readonly string _groupName;
    private readonly IDatabase _db;
    private readonly ConnectionMultiplexer _muxer;
    private readonly IEventHandler _eventHandler;
    
    public RedisEventReader(
        CancellationToken cancellationToken,
        IEventHandler eventHandler,
        string host,
        string streamName,
        string groupName)
    {
        _streamName = streamName;
        _groupName = groupName;
        _eventHandler = eventHandler;
        _cancellationToken = cancellationToken;
        var configurationOptions = ConfigurationOptions.Parse(host);
        configurationOptions.AbortOnConnectFail = false; // Allow retrying
        _muxer = ConnectionMultiplexer.Connect(configurationOptions);
        _db = _muxer.GetDatabase();
    }
    
    public async Task ConsumeMessagesAsync()
    {
        if (!(await _db.KeyExistsAsync(_streamName)) ||
            (await _db.StreamGroupInfoAsync(_streamName)).All(x => x.Name != _groupName))
        {
            await _db.StreamCreateConsumerGroupAsync(_streamName, _groupName, "0-0", true);
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
                    var eventInfo = jsonContent["payload"]["after"];
                    
                    try
                    {
                        DomainEnvelope<IDomainEvent> parsedEvent = EventParser.ParseEvent(eventInfo);
                        await _eventHandler.Handle(parsedEvent);
                    }
                    catch (InvalidOperationException e)
                    {
                        //TODO: Use logger
                        Console.WriteLine("Event Ignored: " + e.Message);
                    }
                }
                
                await Task.Delay(1000, _cancellationToken);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Redis Event Reader stopped!");
                Dispose();
                break;
            }
        }
    }
    
    public void Dispose()
    {
        _muxer.Dispose();
    }
}