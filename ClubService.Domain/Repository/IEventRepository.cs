using ClubService.Domain.Event;

namespace ClubService.Domain.Repository;

public interface IEventRepository
{
    /// <summary>
    ///     Appends an event to the event store and returns the new number of events for the entity.
    /// </summary>
    /// <param name="domainEnvelope">Contains the domain envelope with the event that is appended.</param>
    /// <param name="expectedEventCount">Contains the expected event count before the given domain envelope is appended</param>
    /// <typeparam name="T">is of type IDomainEvent</typeparam>
    /// <returns>The event count after appending the new domain envelope</returns>
    Task<int> Append<T>(DomainEnvelope<T> domainEnvelope, int expectedEventCount) where T : IDomainEvent;

    Task<List<DomainEnvelope<T>>> GetEventsForEntity<T>(Guid entityId, EntityType entityType) where T : IDomainEvent;
}