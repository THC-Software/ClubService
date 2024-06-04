using ClubService.Application.Api;
using ClubService.Domain.Event;
using ClubService.Domain.Event.Member;
using ClubService.Domain.ReadModel;
using ClubService.Domain.Repository;
using ClubService.Domain.Repository.Transaction;

namespace ClubService.Application.EventHandlers.MemberEventHandlers;

public class MemberRegisteredEventHandler(
    IMemberReadModelRepository memberReadModelRepository,
    ITennisClubReadModelRepository tennisClubReadModelRepository, 
    IReadStoreTransactionManager readStoreTransactionManager) : IEventHandler
{
    public async Task Handle(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        if (!Supports(domainEnvelope))
        {
            return;
        }
        
        var memberRegisteredEvent = (MemberRegisteredEvent)domainEnvelope.EventData;
        var tennisClubReadModel =
            await tennisClubReadModelRepository.GetTennisClubById(memberRegisteredEvent.TennisClubId.Id);
        
        if (tennisClubReadModel == null)
        {
            // TODO: Add logging
            Console.WriteLine($"Member with id {domainEnvelope.EntityId} not found!");
            return;
        }
        
        try
        {
            await readStoreTransactionManager.BeginTransactionAsync();
            
            tennisClubReadModel.IncreaseMemberCount();
            await tennisClubReadModelRepository.Update();
            
            var memberReadModel = MemberReadModel.FromDomainEvent(memberRegisteredEvent);
            await memberReadModelRepository.Add(memberReadModel);
            
            await readStoreTransactionManager.CommitTransactionAsync();
        }
        catch (Exception)
        {
            await readStoreTransactionManager.RollbackTransactionAsync();
        }
    }
    
    private static bool Supports(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        return domainEnvelope.EventType.Equals(EventType.MEMBER_REGISTERED);
    }
}