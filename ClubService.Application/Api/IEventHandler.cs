using ClubService.Domain.Event;

namespace ClubService.Application;

public interface IEventHandler
{
    bool SupportsEvent(IDomainEvent domainEvent);
    void Handle(IDomainEvent domainEvent);
}