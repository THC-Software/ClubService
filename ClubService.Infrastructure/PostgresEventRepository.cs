using ClubService.Domain.Event;
using ClubService.Domain.Event.Admin;
using ClubService.Domain.Event.Member;
using ClubService.Domain.Event.SubscriptionTier;
using ClubService.Domain.Event.TennisClub;
using ClubService.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using Npgsql;

namespace ClubService.Infrastructure;

public class PostgresEventRepository(ApplicationDbContext applicationDbContext) : IEventRepository
{
    private const string SelectSqlQuery =
        "SELECT * FROM \"DomainEvent\" WHERE \"entityId\" = @entityId ORDER BY \"timestamp\"";
    
    private IDbContextTransaction _transaction;
    
    public async Task Save<T>(DomainEnvelope<T> domainEnvelope) where T : IDomainEvent
    {
        await applicationDbContext.DomainEvents.AddAsync(new DomainEnvelope<IDomainEvent>(
            domainEnvelope.EventId,
            domainEnvelope.EntityId,
            domainEnvelope.EventType,
            domainEnvelope.EntityType,
            domainEnvelope.Timestamp,
            domainEnvelope.EventData
        ));
        await applicationDbContext.SaveChangesAsync();
    }
    
    public async Task<List<DomainEnvelope<T>>> GetEventsForEntity<T>(Guid entityId) where T : IDomainEvent
    {
        var events = new List<DomainEnvelope<T>>();
        
        await using var command = applicationDbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = SelectSqlQuery;
        command.Parameters.Add(new NpgsqlParameter("@entityId", entityId));
        
        await applicationDbContext.Database.OpenConnectionAsync();
        
        await using var result = await command.ExecuteReaderAsync();
        while (await result.ReadAsync())
        {
            var eventType = (EventType)Enum.Parse(typeof(EventType),
                result.GetString(result.GetOrdinal("EventType")));
            var entityType = (EntityType)Enum.Parse(typeof(EntityType),
                result.GetString(result.GetOrdinal("EntityType")));
            var eventDataJson = result.GetString(result.GetOrdinal("EventData"));
            
            // Determine the type of event and deserialize accordingly
            var eventData = DeserializeEventData<T>(eventType, eventDataJson);
            
            events.Add(new DomainEnvelope<T>(
                result.GetGuid(result.GetOrdinal("EventId")),
                entityId,
                eventType,
                entityType,
                result.GetDateTime(result.GetOrdinal("Timestamp")),
                eventData
            ));
        }
        
        await applicationDbContext.Database.CloseConnectionAsync();
        
        return events;
    }
    
    public async Task BeginTransactionAsync()
    {
        _transaction = await applicationDbContext.Database.BeginTransactionAsync();
    }
    
    public async Task CommitTransactionAsync()
    {
        await _transaction.CommitAsync();
    }
    
    public async Task RollbackTransactionAsync()
    {
        await _transaction.RollbackAsync();
    }
    
    private T DeserializeEventData<T>(EventType eventType, string eventDataJson) where T : IDomainEvent
    {
        switch (eventType)
        {
            case EventType.TENNIS_CLUB_REGISTERED:
                if (JsonConvert.DeserializeObject<TennisClubRegisteredEvent>(eventDataJson) is T
                    tennisClubRegisteredEvent)
                {
                    return tennisClubRegisteredEvent;
                }
                
                break;
            case EventType.MEMBER_REGISTERED:
                if (JsonConvert.DeserializeObject<MemberRegisteredEvent>(eventDataJson) is T memberDomainEvent)
                {
                    return memberDomainEvent;
                }
                
                break;
            case EventType.MEMBER_LIMIT_EXCEEDED:
                if (JsonConvert.DeserializeObject<TennisClubMemberLimitExceededEvent>(eventDataJson) is T
                    tennisClubMemberLimitExceededEvent)
                {
                    return tennisClubMemberLimitExceededEvent;
                }
                
                break;
            case EventType.MEMBER_DELETED:
                if (JsonConvert.DeserializeObject<MemberDeletedEvent>(eventDataJson) is T
                    memberDeletedEvent)
                {
                    return memberDeletedEvent;
                }
                
                break;
            case EventType.ADMIN_REGISTERED:
                if (JsonConvert
                        .DeserializeObject<AdminRegisteredEvent>(eventDataJson) is T adminAccountCreatedEvent)
                {
                    return adminAccountCreatedEvent;
                }
                
                break;
            case EventType.ADMIN_DELETED:
                if (JsonConvert
                        .DeserializeObject<AdminDeletedEvent>(eventDataJson) is T adminAccountDeletedEvent)
                {
                    return adminAccountDeletedEvent;
                }
                
                break;
            case EventType.TENNIS_CLUB_SUBSCRIPTION_TIER_CHANGED:
                if (JsonConvert.DeserializeObject<TennisClubSubscriptionTierChangedEvent>(eventDataJson) is T
                    tennisClubSubscriptionTierChangedEvent)
                {
                    return tennisClubSubscriptionTierChangedEvent;
                }
                
                break;
            case EventType.MEMBER_LOCKED:
                if (JsonConvert.DeserializeObject<MemberLockedEvent>(eventDataJson) is T memberLockedEvent)
                
                {
                    return memberLockedEvent;
                }
                
                break;
            case EventType.MEMBER_UNLOCKED:
                if (JsonConvert.DeserializeObject<MemberUnlockedEvent>(eventDataJson) is T
                    memberUnlockedEvent)
                {
                    return memberUnlockedEvent;
                }
                
                break;
            case EventType.MEMBER_UPDATED:
                if (JsonConvert.DeserializeObject<MemberUpdatedEvent>(eventDataJson) is T
                    memberUpdatedEvent)
                {
                    return memberUpdatedEvent;
                }
                
                break;
            case EventType.TENNIS_CLUB_LOCKED:
                if (JsonConvert.DeserializeObject<TennisClubLockedEvent>(eventDataJson) is T tennisClubLockedEvent)
                {
                    return tennisClubLockedEvent;
                }
                
                break;
            case EventType.TENNIS_CLUB_UNLOCKED:
                if (JsonConvert.DeserializeObject<TennisClubUnlockedEvent>(eventDataJson) is T tennisClubUnlockedEvent)
                {
                    return tennisClubUnlockedEvent;
                }
                
                break;
            case EventType.SUBSCRIPTION_TIER_CREATED:
                if (JsonConvert.DeserializeObject<SubscriptionTierCreatedEvent>(eventDataJson) is T
                    subscriptionTierCreatedEvent)
                {
                    return subscriptionTierCreatedEvent;
                }
                
                break;
            case EventType.TENNIS_CLUB_NAME_CHANGED:
                if (JsonConvert.DeserializeObject<TennisClubNameChangedEvent>(eventDataJson) is T
                    tennisClubNameChangedEvent)
                {
                    return tennisClubNameChangedEvent;
                }
                
                break;
            case EventType.TENNIS_CLUB_DELETED:
                if (JsonConvert.DeserializeObject<TennisClubDeletedEvent>(eventDataJson) is T
                    tennisClubDeletedEvent)
                {
                    return tennisClubDeletedEvent;
                }
                
                break;
            default:
                throw new InvalidOperationException($"Unknown event type: {eventType}");
        }
        
        throw new InvalidCastException($"Failed to cast deserialized object to type {typeof(T)}");
    }
}