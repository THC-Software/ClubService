using ClubService.Application.Api;
using ClubService.Application.Api.Exceptions;
using ClubService.Application.Commands;
using ClubService.Domain.Event.TennisClub;
using ClubService.Domain.Model.Entity;
using ClubService.Domain.Model.Enum;
using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.Repository;
using ClubService.Domain.Repository.Transaction;

namespace ClubService.Application.Impl;

public class RegisterAdminService(
    IEventRepository eventRepository,
    IEventStoreTransactionManager eventStoreTransactionManager) : IRegisterAdminService
{
    public async Task<string> RegisterAdmin(AdminRegisterCommand adminRegisterCommand)
    {
        var tennisClubId = new TennisClubId(new Guid(adminRegisterCommand.TennisClubId));
        var tennisClubDomainEvents =
            await eventRepository.GetEventsForEntity<ITennisClubDomainEvent>(tennisClubId.Id);
        
        if (tennisClubDomainEvents.Count == 0)
        {
            throw new TennisClubNotFoundException(tennisClubId.Id);
        }
        
        var tennisClub = new TennisClub();
        foreach (var domainEvent in tennisClubDomainEvents)
        {
            tennisClub.Apply(domainEvent);
        }
        
        switch (tennisClub.Status)
        {
            case TennisClubStatus.ACTIVE:
                var admin = new Admin();
                var domainEvents = admin.ProcessAdminRegisterCommand(
                    adminRegisterCommand.Username,
                    new FullName(adminRegisterCommand.FirstName, adminRegisterCommand.LastName),
                    new TennisClubId(new Guid(adminRegisterCommand.TennisClubId))
                );
                var expectedEventCount = 0;
                
                await eventStoreTransactionManager.TransactionScope(async () =>
                {
                    foreach (var domainEvent in domainEvents)
                    {
                        admin.Apply(domainEvent);
                        expectedEventCount = await eventRepository.Append(domainEvent, expectedEventCount);
                    }
                });
                
                return admin.AdminId.Id.ToString();
            case TennisClubStatus.LOCKED:
                throw new ConflictException("Tennis club is locked!");
            case TennisClubStatus.DELETED:
                throw new ConflictException("Tennis club already deleted!");
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}