using ClubService.Application.Api;
using ClubService.Application.Api.Exceptions;
using ClubService.Application.Commands;
using ClubService.Domain.Api;
using ClubService.Domain.Event;
using ClubService.Domain.Event.SubscriptionTier;
using ClubService.Domain.Model.Entity;
using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.Repository;
using ClubService.Domain.Repository.Transaction;

namespace ClubService.Application.Impl;

public class RegisterTennisClubService(
    IEventRepository eventRepository,
    ITransactionManager transactionManager,
    IPasswordHasherService passwordHasherService,
    ILoginRepository loginRepository,
    ILoggerService<RegisterTennisClubService> loggerService)
    : IRegisterTennisClubService
{
    public async Task<Guid> RegisterTennisClub(TennisClubRegisterCommand tennisClubRegisterCommand)
    {
        loggerService.LogRegisterTennisClub(tennisClubRegisterCommand.Name,
            tennisClubRegisterCommand.SubscriptionTierId);

        var subscriptionTierId = new SubscriptionTierId(tennisClubRegisterCommand.SubscriptionTierId);
        var subscriptionTierDomainEvents =
            await eventRepository.GetEventsForEntity<ISubscriptionTierDomainEvent>(subscriptionTierId.Id,
                EntityType.SUBSCRIPTION_TIER);

        if (subscriptionTierDomainEvents.Count == 0)
        {
            loggerService.LogSubscriptionTierNotFound(subscriptionTierId.Id);
            throw new SubscriptionTierNotFoundException(subscriptionTierId.Id);
        }

        var tennisClub = new TennisClub();
        var admin = new Admin();

        var tennisClubDomainEvents =
            tennisClub.ProcessTennisClubRegisterCommand(tennisClubRegisterCommand.Name,
                subscriptionTierId);
        var expectedEventCount = 0;

        await transactionManager.TransactionScope(async () =>
        {
            foreach (var domainEvent in tennisClubDomainEvents)
            {
                tennisClub.Apply(domainEvent);
                expectedEventCount = await eventRepository.Append(domainEvent, expectedEventCount);
            }

            var adminDomainEvents = admin.ProcessAdminRegisterCommand(tennisClubRegisterCommand.Username,
                new FullName(tennisClubRegisterCommand.FirstName, tennisClubRegisterCommand.LastName),
                tennisClub.TennisClubId);

            expectedEventCount = 0;
            foreach (var domainEvent in adminDomainEvents)
            {
                admin.Apply(domainEvent);
                expectedEventCount = await eventRepository.Append(domainEvent, expectedEventCount);
            }
        });
        
        await SaveLoginCredentials(admin.AdminId, tennisClubRegisterCommand.Password);
        
        loggerService.LogTennisClubRegistered(tennisClub.TennisClubId.Id);
        loggerService.LogAdminRegistered(admin.AdminId.Id);
        return tennisClub.TennisClubId.Id;
    }

    private async Task SaveLoginCredentials(AdminId adminId, string password)
    {
        var userPassword = UserPassword.Create(adminId.Id, password, passwordHasherService);
        await loginRepository.Add(userPassword);
    }
}