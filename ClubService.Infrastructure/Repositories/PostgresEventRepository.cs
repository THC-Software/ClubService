using System.Data;
using ClubService.Domain.Event;
using ClubService.Domain.Repository;
using ClubService.Infrastructure.DbContexts;
using ClubService.Infrastructure.EventHandling;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Npgsql;

namespace ClubService.Infrastructure.Repositories;

public class PostgresEventRepository(EventStoreDbContext eventStoreDbContext) : IEventRepository
{
    private const string SelectSqlQuery = @"
        SELECT * 
        FROM ""DomainEvent"" 
        WHERE ""entityId"" = @entityId 
        ORDER BY ""timestamp"";
    ";
    
    private const string InsertSqlQuery = @"
        INSERT INTO ""DomainEvent""(""eventId"", ""entityId"", ""eventType"", ""entityType"", ""timestamp"", ""eventData"")
        SELECT @eventId, @entityId, @eventType, @entityType, @timestamp, @eventData
        WHERE (SELECT COUNT(*) FROM ""DomainEvent"" WHERE ""entityId"" = @entityId) = @expectedEventCount;
    ";
    
    public async Task<int> Append<T>(DomainEnvelope<T> domainEnvelope, int expectedEventCount) where T : IDomainEvent
    {
        await eventStoreDbContext.Database.OpenConnectionAsync();
        
        await using var insertCommand = eventStoreDbContext.Database.GetDbConnection().CreateCommand();
        insertCommand.CommandText = InsertSqlQuery;
        
        var jsonSerializedEventData = JsonConvert.SerializeObject(domainEnvelope.EventData);
        insertCommand.Parameters.Add(new NpgsqlParameter("@eventId", domainEnvelope.EventId));
        insertCommand.Parameters.Add(new NpgsqlParameter("@entityId", domainEnvelope.EntityId));
        insertCommand.Parameters.Add(new NpgsqlParameter("@eventType", domainEnvelope.EventType.ToString()));
        insertCommand.Parameters.Add(new NpgsqlParameter("@entityType", domainEnvelope.EntityType.ToString()));
        insertCommand.Parameters.Add(new NpgsqlParameter("@timestamp", domainEnvelope.Timestamp));
        insertCommand.Parameters.Add(new NpgsqlParameter("@eventData", jsonSerializedEventData));
        insertCommand.Parameters.Add(new NpgsqlParameter("@expectedEventCount", expectedEventCount));
        
        var affectedRows = await insertCommand.ExecuteNonQueryAsync();
        await eventStoreDbContext.Database.CloseConnectionAsync();
        
        if (affectedRows == 0)
        {
            throw new DataException(
                $"Expected event count {expectedEventCount} did not match current event count.");
        }
        
        return expectedEventCount + 1;
    }
    
    public async Task<List<DomainEnvelope<T>>> GetEventsForEntity<T>(Guid entityId) where T : IDomainEvent
    {
        var events = new List<DomainEnvelope<T>>();
        
        await using var command = eventStoreDbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = SelectSqlQuery;
        command.Parameters.Add(new NpgsqlParameter("@entityId", entityId));
        
        await eventStoreDbContext.Database.OpenConnectionAsync();
        
        await using var result = await command.ExecuteReaderAsync();
        while (await result.ReadAsync())
        {
            var eventType = (EventType)Enum.Parse(typeof(EventType),
                result.GetString(result.GetOrdinal("EventType")));
            var entityType = (EntityType)Enum.Parse(typeof(EntityType),
                result.GetString(result.GetOrdinal("EntityType")));
            var eventDataJson = result.GetString(result.GetOrdinal("EventData"));
            
            // Determine the type of event and deserialize accordingly
            var eventData = EventDeserializer.DeserializeEventData<T>(eventType, eventDataJson);
            
            events.Add(new DomainEnvelope<T>(
                result.GetGuid(result.GetOrdinal("EventId")),
                entityId,
                eventType,
                entityType,
                result.GetDateTime(result.GetOrdinal("Timestamp")),
                eventData
            ));
        }
        
        await eventStoreDbContext.Database.CloseConnectionAsync();
        
        return events;
    }
}