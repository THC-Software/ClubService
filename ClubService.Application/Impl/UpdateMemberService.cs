using System.Data;
using ClubService.Application.Api;
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
    IEventStoreTransactionManager eventStoreTransactionManager) : IUpdateMemberService
{
    public async Task<string> LockMember(string id)
    {
        var memberId = new MemberId(new Guid(id));
        var existingMemberDomainEvents = await eventRepository.GetEventsForEntity<IMemberDomainEvent>(memberId.Id);
        
        if (existingMemberDomainEvents.Count == 0)
        {
            throw new MemberNotFoundException(memberId.Id);
        }
        
        var member = new Member();
        foreach (var domainEvent in existingMemberDomainEvents)
        {
            member.Apply(domainEvent);
        }
        
        var tennisClubDomainEvents =
            await eventRepository.GetEventsForEntity<ITennisClubDomainEvent>(member.TennisClubId.Id);
        
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
                    throw new ConflictException(ex.Message, ex);
                }
                
                return id;
            case TennisClubStatus.LOCKED:
                throw new ConflictException("Tennis club is locked!");
            case TennisClubStatus.DELETED:
                throw new ConflictException("Tennis club already deleted!");
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public async Task<string> UnlockMember(string id)
    {
        var memberId = new MemberId(new Guid(id));
        var existingMemberDomainEvents = await eventRepository.GetEventsForEntity<IMemberDomainEvent>(memberId.Id);
        
        if (existingMemberDomainEvents.Count == 0)
        {
            throw new MemberNotFoundException(memberId.Id);
        }
        
        var member = new Member();
        foreach (var domainEvent in existingMemberDomainEvents)
        {
            member.Apply(domainEvent);
        }
        
        var tennisClubDomainEvents =
            await eventRepository.GetEventsForEntity<ITennisClubDomainEvent>(member.TennisClubId.Id);
        
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
                    
                    foreach (var domainEvent in domainEvents)
                    {
                        member.Apply(domainEvent);
                        expectedEventCount = await eventRepository.Append(domainEvent, expectedEventCount);
                    }
                }
                catch (InvalidOperationException ex)
                {
                    throw new ConflictException(ex.Message, ex);
                }
                catch (DataException ex)
                {
                    throw new ConcurrencyException(ex.Message, ex);
                }
                
                return id;
            case TennisClubStatus.LOCKED:
                throw new ConflictException("Tennis club is locked!");
            case TennisClubStatus.DELETED:
                throw new ConflictException("Tennis club already deleted!");
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public async Task<string> ChangeFullName(string id, string firstName, string lastName)
    {
        var memberId = Guid.Parse(id);
        var existingMemberDomainEvents = await eventRepository.GetEventsForEntity<IMemberDomainEvent>(memberId);
        
        if (existingMemberDomainEvents.Count == 0)
        {
            throw new MemberNotFoundException(memberId);
        }
        
        var member = new Member();
        
        foreach (var domainEvent in existingMemberDomainEvents)
        {
            member.Apply(domainEvent);
        }
        
        var tennisClubDomainEvents =
            await eventRepository.GetEventsForEntity<ITennisClubDomainEvent>(member.TennisClubId.Id);
        
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
                    
                    foreach (var domainEvent in domainEvents)
                    {
                        member.Apply(domainEvent);
                        expectedEventCount = await eventRepository.Append(domainEvent, expectedEventCount);
                    }
                }
                catch (InvalidOperationException ex)
                {
                    throw new ConflictException(ex.Message, ex);
                }
                catch (DataException ex)
                {
                    throw new ConcurrencyException(ex.Message, ex);
                }
                
                return id;
            case TennisClubStatus.LOCKED:
                throw new ConflictException("Tennis club is locked!");
            case TennisClubStatus.DELETED:
                throw new ConflictException("Tennis club already deleted!");
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public async Task<string> ChangeEmail(string id, string email)
    {
        var memberId = Guid.Parse(id);
        var existingMemberDomainEvents = await eventRepository.GetEventsForEntity<IMemberDomainEvent>(memberId);
        
        if (existingMemberDomainEvents.Count == 0)
        {
            throw new MemberNotFoundException(memberId);
        }
        
        var member = new Member();
        
        foreach (var domainEvent in existingMemberDomainEvents)
        {
            member.Apply(domainEvent);
        }
        
        var tennisClubDomainEvents =
            await eventRepository.GetEventsForEntity<ITennisClubDomainEvent>(member.TennisClubId.Id);
        
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
                    
                    foreach (var domainEvent in domainEvents)
                    {
                        member.Apply(domainEvent);
                        expectedEventCount = await eventRepository.Append(domainEvent, expectedEventCount);
                    }
                }
                catch (InvalidOperationException ex)
                {
                    throw new ConflictException(ex.Message, ex);
                }
                catch (DataException ex)
                {
                    throw new ConcurrencyException(ex.Message, ex);
                }
                
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