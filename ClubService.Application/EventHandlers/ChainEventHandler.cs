using ClubService.Application.Api;
using ClubService.Domain.Event;
using ClubService.Domain.ReadModel;
using ClubService.Domain.Repository;
using ClubService.Domain.Repository.Transaction;

namespace ClubService.Application.EventHandlers;

public class ChainEventHandler(
    IEnumerable<IEventHandler> eventHandlers,
    IProcessedEventRepository processedEventRepository,
    ITransactionManager transactionManager,
    ILoggerService<ChainEventHandler> loggerService) : IEventHandler
{
    public async Task Handle(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        loggerService.LogHandleEvent(domainEnvelope);

        await transactionManager.TransactionScope(async () =>
        {
            foreach (var eventHandler in eventHandlers)
            {
                await eventHandler.Handle(domainEnvelope);
            }

            var processedEvent = new ProcessedEvent(domainEnvelope.EventId);
            await processedEventRepository.Add(processedEvent);
        });
    }
}