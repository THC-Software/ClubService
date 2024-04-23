using ClubService.Domain.Event;
using ClubService.Domain.Repository;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace ClubService.Infrastructure;

public class RedisEventPublisher(string redisConnectionString) : IEventPublisher
{
    private readonly ConnectionMultiplexer _redisConnection = ConnectionMultiplexer.Connect(redisConnectionString);

    public async Task PublishEvent<T>(DomainEnvelope<T> domainEnvelope) where T : IDomainEvent
    {
        var database = _redisConnection.GetDatabase();

        var serializedEvent = JsonConvert.SerializeObject(domainEnvelope);

        await database.StreamAddAsync("tennisclubservice", [
            new NameValueEntry(domainEnvelope.EventType.ToString(), serializedEvent)
        ]);
    }
}