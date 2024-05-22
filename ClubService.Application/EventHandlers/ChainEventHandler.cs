using ClubService.Domain.Event;

namespace ClubService.Application;

public class ChainEventHandler : IEventHandler
{
    private List<IEventHandler> EventHandlers { get;  set; } = new();

    //TODO Ask Daniel about supports event method because CEH should support every event
    public bool SupportsEvent(IDomainEvent domainEvent)
    {
        return true;
    }

    public void Handle(IDomainEvent domainEvent)
    {
        foreach (var eventHandler in EventHandlers)
        {
            if (eventHandler.SupportsEvent(domainEvent))
            {
                eventHandler.Handle(domainEvent);
            }
        }
    }

    public void RegisterEventHandler(IEventHandler eventHandler)
    {
        EventHandlers.Add(eventHandler);
    }
}