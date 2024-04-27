using ClubService.Domain.Event;
using ClubService.Domain.Repository;

namespace ClubService.Infrastructure;

public class PostgresEventRepository : IEventRepository
{
    public void Save<T>(DomainEnvelope<T> domainEnvelope) where T : IDomainEvent
    {
        throw new NotImplementedException();
    }
}