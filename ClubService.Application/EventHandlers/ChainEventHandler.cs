using ClubService.Application.Api;
using ClubService.Domain.Event;
using ClubService.Domain.ReadModel;
using ClubService.Domain.Repository;

namespace ClubService.Application.EventHandlers;

public class ChainEventHandler(
    IEnumerable<IEventHandler> eventHandlers,
    IProcessedEventRepository processedEventRepository,
    ILoggerService<ChainEventHandler> loggerService) : IEventHandler
{
    public async Task Handle(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        loggerService.LogHandleEvent(domainEnvelope);
        var processedEvents = await processedEventRepository.GetAllProcessedEvents();

        if (processedEvents.Exists(pe => pe.EventId == domainEnvelope.EventId))
        {
            return;
        }

        foreach (var eventHandler in eventHandlers)
        {
            await eventHandler.Handle(domainEnvelope);
        }

        var processedEvent = new ProcessedEvent(domainEnvelope.EventId);
        await processedEventRepository.Add(processedEvent);
    }
}