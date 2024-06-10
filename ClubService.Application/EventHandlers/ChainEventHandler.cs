using ClubService.Application.Api;
using ClubService.Domain.Event;
using ClubService.Domain.ReadModel;
using ClubService.Domain.Repository;

namespace ClubService.Application.EventHandlers;

public class ChainEventHandler(IProcessedEventRepository processedEventRepository) : IEventHandler
{
    private List<IEventHandler> EventHandlers { get; } = new();
    
    public async Task Handle(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        var processedEvents = await processedEventRepository.GetAllProcessedEvents();
        
        if (processedEvents.Exists(pe => pe.EventId == domainEnvelope.EventId))
        {
            return;
        }
        
        foreach (var eventHandler in EventHandlers)
        {
            Console.WriteLine("Handling event in " + eventHandler.GetType().Name);
            await eventHandler.Handle(domainEnvelope);
        }
        
        var processedEvent = new ProcessedEvent(domainEnvelope.EventId);
        await processedEventRepository.Add(processedEvent);
    }
    
    public void RegisterEventHandler(IEventHandler eventHandler)
    {
        EventHandlers.Add(eventHandler);
    }
}