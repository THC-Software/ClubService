using ClubService.Domain.Event;
using ClubService.Domain.Event.Admin;
using ClubService.Domain.Event.Member;
using ClubService.Domain.Event.TennisClub;
using ClubService.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Npgsql;

namespace ClubService.Infrastructure;

public class PostgresEventRepository(ApplicationDbContext applicationDbContext) : IEventRepository
{
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

    public List<DomainEnvelope<IDomainEvent>> GetEventsForEntity<T>(Guid entityId) where T : IDomainEvent
    {
        var sql = "SELECT * FROM \"DomainEvent\" WHERE \"entityId\" = @entityId";
        var events = new List<DomainEnvelope<IDomainEvent>>();

        using (var command = applicationDbContext.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = sql;
            command.Parameters.Add(new NpgsqlParameter("@entityId", entityId));

            applicationDbContext.Database.OpenConnection();

            using (var result = command.ExecuteReader())
            {
                while (result.Read())
                {
                    var eventType = (EventType)Enum.Parse(typeof(EventType),
                        result.GetString(result.GetOrdinal("EventType")));
                    var entityType = (EntityType)Enum.Parse(typeof(EntityType),
                        result.GetString(result.GetOrdinal("EntityType")));
                    var eventDataJson = result.GetString(result.GetOrdinal("EventData"));

                    // Determine the type of event and deserialize accordingly
                    var eventData = DeserializeEventData(eventType, eventDataJson);

                    events.Add(new DomainEnvelope<IDomainEvent>(
                        result.GetGuid(result.GetOrdinal("EventId")),
                        entityId,
                        eventType,
                        entityType,
                        result.GetDateTime(result.GetOrdinal("Timestamp")),
                        eventData
                    ));
                }
            }
        }

        return events;
    }

    private IDomainEvent DeserializeEventData(EventType eventType, string eventDataJson)
    {
        switch (eventType)
        {
            case EventType.TENNIS_CLUB_REGISTERED:
                return JsonConvert.DeserializeObject<TennisClubRegisteredEvent>(eventDataJson);
            case EventType.MEMBER_ACCOUNT_CREATED:
                return JsonConvert.DeserializeObject<MemberAccountDomainEvent>(eventDataJson);
            case EventType.MEMBER_ACCOUNT_LIMIT_EXCEEDED:
                return JsonConvert.DeserializeObject<TennisClubMemberAccountLimitExceededEvent>(eventDataJson);
            case EventType.MEMBER_ACCOUNT_DELETED:
                return JsonConvert.DeserializeObject<MemberAccountDeletedEvent>(eventDataJson);
            case EventType.ADMIN_ACCOUNT_CREATED:
                return JsonConvert.DeserializeObject<AdminAccountCreatedEvent>(eventDataJson);
            case EventType.ADMIN_ACCOUNT_DELETED:
                return JsonConvert.DeserializeObject<AdminAccountDeletedEvent>(eventDataJson);
            case EventType.TENNIS_CLUB_SUBSCRIPTION_TIER_CHANGED:
                return JsonConvert.DeserializeObject<TennisClubSubscriptionTierChangedEvent>(eventDataJson);
            case EventType.MEMBER_ACCOUNT_LOCKED:
                return JsonConvert.DeserializeObject<MemberAccountLockedEvent>(eventDataJson);
            case EventType.MEMBER_ACCOUNT_UNLOCKED:
                return JsonConvert.DeserializeObject<MemberAccountUnlockedEvent>(eventDataJson);
            case EventType.MEMBER_ACCOUNT_UPDATED:
                return JsonConvert.DeserializeObject<MemberAccountUpdatedEvent>(eventDataJson);
            case EventType.TENNIS_CLUB_LOCKED:
                return JsonConvert.DeserializeObject<TennisClubLockedEvent>(eventDataJson);
            case EventType.TENNIS_CLUB_UNLOCKED:
                return JsonConvert.DeserializeObject<TennisClubUnlockedEvent>(eventDataJson);
            default:
                throw new InvalidOperationException($"Unknown event type: {eventType}");
        }
    }
}