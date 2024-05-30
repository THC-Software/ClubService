using ClubService.Domain.Event;
using ClubService.Domain.Repository;
using ClubService.Infrastructure.DbContexts;
using ClubService.Infrastructure.EventHandling;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using Npgsql;

namespace ClubService.Infrastructure;

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
        VALUES(@eventId, @entityId, @eventType, @entityType, @timestamp, @eventData);
    ";
    
    private IDbContextTransaction _transaction;
    
    public async Task Append<T>(DomainEnvelope<T> domainEnvelope) where T : IDomainEvent
    {
        await using var command = eventStoreDbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = InsertSqlQuery;
        
        var jsonSerializedEventData = JsonConvert.SerializeObject(domainEnvelope.EventData);
        command.Parameters.Add(new NpgsqlParameter("@eventId", domainEnvelope.EventId));
        command.Parameters.Add(new NpgsqlParameter("@entityId", domainEnvelope.EntityId));
        command.Parameters.Add(new NpgsqlParameter("@eventType", domainEnvelope.EventType.ToString()));
        command.Parameters.Add(new NpgsqlParameter("@entityType", domainEnvelope.EntityType.ToString()));
        command.Parameters.Add(new NpgsqlParameter("@timestamp", domainEnvelope.Timestamp));
        command.Parameters.Add(new NpgsqlParameter("@eventData", jsonSerializedEventData));
        
        await eventStoreDbContext.Database.OpenConnectionAsync();
        await command.ExecuteNonQueryAsync();
        await eventStoreDbContext.Database.CloseConnectionAsync();
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
    
    public async Task BeginTransactionAsync()
    {
        _transaction = await eventStoreDbContext.Database.BeginTransactionAsync();
    }
    
    public async Task CommitTransactionAsync()
    {
        await _transaction.CommitAsync();
    }
    
    public async Task RollbackTransactionAsync()
    {
        await _transaction.RollbackAsync();
    }
}