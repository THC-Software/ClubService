using System.Text.Json.Nodes;
using ClubService.Application.Api;
using ClubService.Domain.Event;
using ClubService.Infrastructure.Api;
using StackExchange.Redis;

namespace ClubService.Infrastructure;

public class RedisEventReader : IEventReader
{
    private readonly CancellationToken _cancellationToken;
    private const string StreamName = "club_service_events.public.DomainEvent";
    private const string GroupName = "club_service_events.domain.events.group";
    private readonly IDatabase _db;
    private readonly ConnectionMultiplexer _muxer;
    private readonly IEventHandler _eventHandler;
    
    public RedisEventReader(CancellationToken cancellationToken, IEventHandler eventHandler)
    {
        _eventHandler = eventHandler;
        _cancellationToken = cancellationToken;
        var configurationOptions = ConfigurationOptions.Parse("localhost");
        configurationOptions.AbortOnConnectFail = false; // Allow retrying
        _muxer = ConnectionMultiplexer.Connect(configurationOptions);
        _db = _muxer.GetDatabase();
    }
    
    public async Task ConsumeMessagesAsync()
    {
        if (!(await _db.KeyExistsAsync(StreamName)) ||
            (await _db.StreamGroupInfoAsync(StreamName)).All(x => x.Name != GroupName))
        {
            await _db.StreamCreateConsumerGroupAsync(StreamName, GroupName, "0-0", true);
        }
        
        var id = string.Empty;
        while (!_cancellationToken.IsCancellationRequested)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    await _db.StreamAcknowledgeAsync(StreamName, GroupName, id);
                    id = string.Empty;
                }
                
                var result = await _db.StreamReadGroupAsync(StreamName, GroupName, "pos-member", ">", 1);
                if (result.Any())
                {
                    var streamEntry = result.First();
                    var dict = streamEntry.Values.ToDictionary(x => x.Name.ToString(), x => x.Value.ToString());
                    var jsonContent = JsonNode.Parse(dict.Values.First());
                    var eventInfo = jsonContent["payload"]["after"];
                    
                    DomainEnvelope<IDomainEvent> parsedEvent = EventParser.ParseEvent(eventInfo);
                    _eventHandler.Handle(parsedEvent);
                }
                
                await Task.Delay(1000, _cancellationToken);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Redis Event Reader stopped!");
                break;
            }
        }
    }
    
    public void Dispose()
    {
        _muxer.Dispose();
    }
}