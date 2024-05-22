using ClubService.Application.Api;
using ClubService.Domain.Event;

namespace ClubService.Application.EventHandlers;

public class ChainEventHandler : IEventHandler
{
    private List<IEventHandler> EventHandlers { get; set; } = new();

    public void Handle(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        foreach (var eventHandler in EventHandlers)
        {
            eventHandler.Handle(domainEnvelope);
        }
    }

    public void RegisterEventHandler(IEventHandler eventHandler)
    {
        EventHandlers.Add(eventHandler);
    }
}