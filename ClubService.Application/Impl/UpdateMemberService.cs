﻿using ClubService.Application.Api;
using ClubService.Application.Api.Exceptions;
using ClubService.Domain.Event.Member;
using ClubService.Domain.Event.TennisClub;
using ClubService.Domain.Model.Entity;
using ClubService.Domain.Model.Enum;
using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.Repository;
using ClubService.Domain.Repository.Transaction;

namespace ClubService.Application.Impl;

public class UpdateMemberService(
    IEventRepository eventRepository,
    IEventStoreTransactionManager eventStoreTransactionManager,
    ILoggerService<UpdateMemberService> loggerService) : IUpdateMemberService
{
    public async Task<Guid> LockMember(Guid id)
    {
        loggerService.LogLockMember(id);

        var memberId = new MemberId(id);
        var existingMemberDomainEvents = await eventRepository.GetEventsForEntity<IMemberDomainEvent>(memberId.Id);

        if (existingMemberDomainEvents.Count == 0)
        {
            loggerService.LogMemberNotFound(id);
            throw new MemberNotFoundException(memberId.Id);
        }

        var member = new Member();
        foreach (var domainEvent in existingMemberDomainEvents)
        {
            member.Apply(domainEvent);
        }

        var tennisClubDomainEvents =
            await eventRepository.GetEventsForEntity<ITennisClubDomainEvent>(member.TennisClubId.Id);

        // TODO: Check if tennisClubDomainEvents.Count > 0

        var tennisClub = new TennisClub();
        foreach (var domainEvent in tennisClubDomainEvents)
        {
            tennisClub.Apply(domainEvent);
        }

        switch (tennisClub.Status)
        {
            case TennisClubStatus.ACTIVE:
                try
                {
                    var domainEvents = member.ProcessMemberLockCommand();
                    var expectedEventCount = existingMemberDomainEvents.Count;

                    await eventStoreTransactionManager.TransactionScope(async () =>
                    {
                        foreach (var domainEvent in domainEvents)
                        {
                            member.Apply(domainEvent);
                            expectedEventCount = await eventRepository.Append(domainEvent, expectedEventCount);
                        }
                    });
                }
                catch (InvalidOperationException ex)
                {
                    loggerService.LogInvalidOperationException(ex);
                    throw new ConflictException(ex.Message, ex);
                }

                loggerService.LogMemberLocked(id);
                return id;
            case TennisClubStatus.LOCKED:
                throw new ConflictException("Tennis club is locked!");
            case TennisClubStatus.DELETED:
                throw new ConflictException("Tennis club already deleted!");
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public async Task<Guid> UnlockMember(Guid id)
    {
        loggerService.LogUnlockMember(id);

        var memberId = new MemberId(id);
        var existingMemberDomainEvents = await eventRepository.GetEventsForEntity<IMemberDomainEvent>(memberId.Id);

        if (existingMemberDomainEvents.Count == 0)
        {
            loggerService.LogMemberNotFound(id);
            throw new MemberNotFoundException(memberId.Id);
        }

        var member = new Member();
        foreach (var domainEvent in existingMemberDomainEvents)
        {
            member.Apply(domainEvent);
        }

        var tennisClubDomainEvents =
            await eventRepository.GetEventsForEntity<ITennisClubDomainEvent>(member.TennisClubId.Id);

        // TODO: Check if tennisClubDomainEvents.Count > 0

        var tennisClub = new TennisClub();
        foreach (var domainEvent in tennisClubDomainEvents)
        {
            tennisClub.Apply(domainEvent);
        }

        switch (tennisClub.Status)
        {
            case TennisClubStatus.ACTIVE:
                try
                {
                    var domainEvents = member.ProcessMemberUnlockCommand();
                    var expectedEventCount = existingMemberDomainEvents.Count;

                    await eventStoreTransactionManager.TransactionScope(async () =>
                    {
                        foreach (var domainEvent in domainEvents)
                        {
                            member.Apply(domainEvent);
                            expectedEventCount = await eventRepository.Append(domainEvent, expectedEventCount);
                        }
                    });
                }
                catch (InvalidOperationException ex)
                {
                    loggerService.LogInvalidOperationException(ex);
                    throw new ConflictException(ex.Message, ex);
                }

                loggerService.LogMemberUnlocked(id);
                return id;
            case TennisClubStatus.LOCKED:
                throw new ConflictException("Tennis club is locked!");
            case TennisClubStatus.DELETED:
                throw new ConflictException("Tennis club already deleted!");
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public async Task<Guid> ChangeFullName(Guid id, string firstName, string lastName)
    {
        loggerService.LogMemberChangeFullName(id, firstName, lastName);

        var memberId = id;
        var existingMemberDomainEvents = await eventRepository.GetEventsForEntity<IMemberDomainEvent>(memberId);

        if (existingMemberDomainEvents.Count == 0)
        {
            loggerService.LogMemberNotFound(id);
            throw new MemberNotFoundException(memberId);
        }

        var member = new Member();

        foreach (var domainEvent in existingMemberDomainEvents)
        {
            member.Apply(domainEvent);
        }

        var tennisClubDomainEvents =
            await eventRepository.GetEventsForEntity<ITennisClubDomainEvent>(member.TennisClubId.Id);

        // TODO: Check if tennisClubDomainEvents.Count > 0

        var tennisClub = new TennisClub();
        foreach (var domainEvent in tennisClubDomainEvents)
        {
            tennisClub.Apply(domainEvent);
        }

        switch (tennisClub.Status)
        {
            case TennisClubStatus.ACTIVE:
                try
                {
                    var domainEvents = member.ProcessMemberChangeFullNameCommand(new FullName(firstName, lastName));
                    var expectedEventCount = existingMemberDomainEvents.Count;

                    await eventStoreTransactionManager.TransactionScope(async () =>
                    {
                        foreach (var domainEvent in domainEvents)
                        {
                            member.Apply(domainEvent);
                            expectedEventCount = await eventRepository.Append(domainEvent, expectedEventCount);
                        }
                    });
                }
                catch (InvalidOperationException ex)
                {
                    loggerService.LogInvalidOperationException(ex);
                    throw new ConflictException(ex.Message, ex);
                }

                loggerService.LogMemberFullNameChanged(id);
                return id;
            case TennisClubStatus.LOCKED:
                throw new ConflictException("Tennis club is locked!");
            case TennisClubStatus.DELETED:
                throw new ConflictException("Tennis club already deleted!");
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public async Task<Guid> ChangeEmail(Guid id, string email)
    {
        loggerService.LogMemberChangeEmail(id, email);

        var memberId = id;
        var existingMemberDomainEvents = await eventRepository.GetEventsForEntity<IMemberDomainEvent>(memberId);

        if (existingMemberDomainEvents.Count == 0)
        {
            loggerService.LogMemberNotFound(id);
            throw new MemberNotFoundException(memberId);
        }

        var member = new Member();

        foreach (var domainEvent in existingMemberDomainEvents)
        {
            member.Apply(domainEvent);
        }

        var tennisClubDomainEvents =
            await eventRepository.GetEventsForEntity<ITennisClubDomainEvent>(member.TennisClubId.Id);

        // TODO: Check if tennisClubDomainEvents.Count > 0

        var tennisClub = new TennisClub();
        foreach (var domainEvent in tennisClubDomainEvents)
        {
            tennisClub.Apply(domainEvent);
        }

        switch (tennisClub.Status)
        {
            case TennisClubStatus.ACTIVE:
                try
                {
                    var domainEvents = member.ProcessMemberChangeEmailCommand(email);
                    var expectedEventCount = existingMemberDomainEvents.Count;

                    await eventStoreTransactionManager.TransactionScope(async () =>
                    {
                        foreach (var domainEvent in domainEvents)
                        {
                            member.Apply(domainEvent);
                            expectedEventCount = await eventRepository.Append(domainEvent, expectedEventCount);
                        }
                    });
                }
                catch (InvalidOperationException ex)
                {
                    loggerService.LogInvalidOperationException(ex);
                    throw new ConflictException(ex.Message, ex);
                }

                loggerService.LogMemberEmailChanged(id);
                return id;
            case TennisClubStatus.LOCKED:
                throw new ConflictException("Tennis club is locked!");
            case TennisClubStatus.DELETED:
                throw new ConflictException("Tennis club already deleted!");
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}