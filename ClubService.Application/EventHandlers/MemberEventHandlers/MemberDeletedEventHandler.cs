using ClubService.Application.Api;
using ClubService.Domain.Event;
using ClubService.Domain.Repository;

namespace ClubService.Application.EventHandlers.MemberEventHandlers;

public class MemberDeletedEventHandler(
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
        
        var memberReadModel = await memberReadModelRepository.GetMemberById(domainEnvelope.EntityId);
        
        if (memberReadModel == null)
        {
            // TODO: Add logging
            Console.WriteLine($"Member with id {domainEnvelope.EntityId} not found!");
            return;
        }
        
        var tennisClubReadModel =
            await tennisClubReadModelRepository.GetTennisClubById(memberReadModel.TennisClubId.Id);
        
        if (tennisClubReadModel == null)
        {
            // TODO: Add logging
            Console.WriteLine($"Tennis club with id {domainEnvelope.EntityId} not found!");
            return;
        }
        
        try
        {
            await readStoreTransactionManager.BeginTransactionAsync();
            
            tennisClubReadModel.DecreaseMemberCount();
            await tennisClubReadModelRepository.Update();
            
            await memberReadModelRepository.Delete(memberReadModel);
            
            await readStoreTransactionManager.CommitTransactionAsync();
        }
        catch (Exception)
        {
            await readStoreTransactionManager.RollbackTransactionAsync();
        }
    }
    
    private static bool Supports(DomainEnvelope<IDomainEvent> domainEnvelope)
    {
        return domainEnvelope.EventType.Equals(EventType.MEMBER_DELETED);
    }
}