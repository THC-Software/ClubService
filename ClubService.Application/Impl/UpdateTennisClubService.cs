using ClubService.Application.Api;
using ClubService.Application.Api.Exceptions;
using ClubService.Application.Commands;
using ClubService.Domain.Event.SubscriptionTier;
using ClubService.Domain.Event.TennisClub;
using ClubService.Domain.Model.Entity;
using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.Repository;
using ClubService.Domain.Repository.Transaction;
using ValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace ClubService.Application.Impl;

public class UpdateTennisClubService(
    IEventRepository eventRepository,
    IEventStoreTransactionManager eventStoreTransactionManager,
    ILoggerService<UpdateTennisClubService> loggerService) : IUpdateTennisClubService
{
    public async Task<Guid> LockTennisClub(Guid id)
    {
        loggerService.LogLockTennisClub(id);

        var tennisClubId = new TennisClubId(id);
        var tennisClub = new TennisClub();

        var existingTennisClubDomainEvents =
            await eventRepository.GetEventsForEntity<ITennisClubDomainEvent>(tennisClubId.Id);

        if (existingTennisClubDomainEvents.Count == 0)
        {
            loggerService.LogTennisClubNotFound(id);
            throw new TennisClubNotFoundException(tennisClubId.Id);
        }

        foreach (var domainEvent in existingTennisClubDomainEvents)
        {
            tennisClub.Apply(domainEvent);
        }

        try
        {
            var domainEvents = tennisClub.ProcessTennisClubLockCommand();
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

        loggerService.LogTennisClubLocked(id);
        return id;
    }

    public async Task<Guid> UnlockTennisClub(Guid id)
    {
        loggerService.LogUnlockTennisClub(id);

        var tennisClubId = new TennisClubId(id);
        var tennisClub = new TennisClub();

        var existingTennisClubDomainEvents =
            await eventRepository.GetEventsForEntity<ITennisClubDomainEvent>(tennisClubId.Id);

        if (existingTennisClubDomainEvents.Count == 0)
        {
            loggerService.LogTennisClubNotFound(id);
            throw new TennisClubNotFoundException(tennisClubId.Id);
        }

        foreach (var domainEvent in existingTennisClubDomainEvents)
        {
            tennisClub.Apply(domainEvent);
        }

        try
        {
            var domainEvents = tennisClub.ProcessTennisClubUnlockCommand();
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

        loggerService.LogTennisClubUnlocked(id);
        return id;
    }

    public async Task<Guid> UpdateTennisClub(Guid id, TennisClubUpdateCommand tennisClubUpdateCommand)
    {
        loggerService.LogUpdateTennisClub(id, tennisClubUpdateCommand.Name, tennisClubUpdateCommand.SubscriptionTierId);

        if (string.IsNullOrWhiteSpace(tennisClubUpdateCommand.Name) &&
            tennisClubUpdateCommand.SubscriptionTierId == null)
        {
            var validationMessage = "You have to either provide a name or a subscription tier id!";
            loggerService.LogValidationFailure(validationMessage);
            throw new ValidationException(validationMessage);
        }

        var tennisClubId = new TennisClubId(id);

        var existingTennisClubDomainEvents =
            await eventRepository.GetEventsForEntity<ITennisClubDomainEvent>(tennisClubId.Id);

        if (existingTennisClubDomainEvents.Count == 0)
        {
            loggerService.LogTennisClubNotFound(id);
            throw new TennisClubNotFoundException(tennisClubId.Id);
        }

        SubscriptionTierId? subscriptionTierId = null;
        if (tennisClubUpdateCommand.SubscriptionTierId != null)
        {
            subscriptionTierId = new SubscriptionTierId((Guid)tennisClubUpdateCommand.SubscriptionTierId);

            var existingSubscriptionTierDomainEvents =
                await eventRepository.GetEventsForEntity<ISubscriptionTierDomainEvent>(subscriptionTierId.Id);

            if (existingSubscriptionTierDomainEvents.Count == 0)
            {
                loggerService.LogSubscriptionTierNotFound(subscriptionTierId.Id);
                throw new SubscriptionTierNotFoundException(subscriptionTierId.Id);
            }
        }

        var tennisClub = new TennisClub();
        foreach (var domainEvent in existingTennisClubDomainEvents)
        {
            tennisClub.Apply(domainEvent);
        }

        try
        {
            var domainEvents =
                tennisClub.ProcessTennisClubUpdateCommand(tennisClubUpdateCommand.Name, subscriptionTierId);
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

        loggerService.LogTennisClubUpdated(id);
        return id;
    }
}