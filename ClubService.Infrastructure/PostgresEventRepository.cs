using ClubService.Domain.Event;
using ClubService.Domain.Repository;

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
}