using ClubService.Application.Api;
using ClubService.Domain.Event;

namespace ClubService.Application.EventHandlers;

public class ChainEventHandler : IEventHandler
{
    private List<IEventHandler> EventHandlers { get; } = new();
    
    public async Task Handle(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        Console.WriteLine(domainEnvelope.ToString());
        foreach (var eventHandler in EventHandlers)
        {
            await eventHandler.Handle(domainEnvelope);
        }
    }
    
    public void RegisterEventHandler(IEventHandler eventHandler)
    {
        EventHandlers.Add(eventHandler);
    }
}