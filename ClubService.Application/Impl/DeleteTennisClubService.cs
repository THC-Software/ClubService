using ClubService.Application.Api;
using ClubService.Application.Api.Exceptions;
using ClubService.Domain.Event.TennisClub;
using ClubService.Domain.Model.Entity;
using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.Repository;
using ClubService.Domain.Repository.Transaction;

namespace ClubService.Application.Impl;

public class DeleteTennisClubService(
    IEventRepository eventRepository,
    IEventStoreTransactionManager eventStoreTransactionManager,
    ILoggerService<DeleteTennisClubService> loggerService) : IDeleteTennisClubService
{
    public async Task<Guid> DeleteTennisClub(Guid id)
    {
        loggerService.LogDeleteTennisClub(id);
        
        var tennisClubId = new TennisClubId(id);
        
        var existingTennisClubDomainEvents =
            await eventRepository.GetEventsForEntity<ITennisClubDomainEvent>(tennisClubId.Id);
        
        if (existingTennisClubDomainEvents.Count == 0)
        {
            loggerService.LogTennisClubNotFound(id);
            throw new TennisClubNotFoundException(tennisClubId.Id);
        }
        
        var tennisClub = new TennisClub();
        foreach (var domainEvent in existingTennisClubDomainEvents)
        {
            tennisClub.Apply(domainEvent);
        }
        
        // we decided to only delete the corresponding admins and members on the read side
        try
        {
            var domainEvents = tennisClub.ProcessTennisClubDeleteCommand();
            var expectedEventCount = existingTennisClubDomainEvents.Count;
            
            await eventStoreTransactionManager.TransactionScope(async () =>
            {
                foreach (var domainEvent in domainEvents)
                {
                    tennisClub.Apply(domainEvent);
                    expectedEventCount = await eventRepository.Append(domainEvent, expectedEventCount);
                }
            });
        }
        catch (InvalidOperationException ex)
        {
            loggerService.LogInvalidOperationException(ex);
            throw new ConflictException(ex.Message, ex);
        }
        
        loggerService.LogTennisClubDeleted(id);
        return tennisClub.TennisClubId.Id;
    }
}