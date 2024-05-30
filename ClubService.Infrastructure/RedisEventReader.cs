using ClubService.Infrastructure.Api;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace ClubService.Infrastructure;

public class RedisEventReader : IEventReader
{
    private readonly CancellationToken _cancellationToken;
    private const string StreamName = "club_service_events.public.DomainEvent";
    private const string GroupName = "club_service_events.domain.events.group";
    private readonly IDatabase _db;

    public RedisEventReader(CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;
        var configurationOptions = ConfigurationOptions.Parse("localhost");
        configurationOptions.AbortOnConnectFail = false; // Allow retrying
        var muxer = ConnectionMultiplexer.Connect(configurationOptions);
        _db = muxer.GetDatabase();
    }

    public async Task ConsumeMessagesAsync()
    {
        if (!(await _db.KeyExistsAsync(StreamName)) ||
            (await _db.StreamGroupInfoAsync(StreamName)).All(x=>x.Name!=GroupName))
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
                    id = streamEntry.Id;
                    //CALL CHAIN EVENT HANDLER
                    // var parsedEvent = ParseEvent(streamEntry);
                    // if (parsedEvent == null)
                    //     continue;
                    // await memberRepository.UpdateEntityAsync(parsedEvent);
                }
                await Task.Delay(1000, _cancellationToken);
            }
            catch (OperationCanceledException )
            {
                Console.WriteLine("Redis Event Reader stopped!");
                break;
            }
        }
    }
}