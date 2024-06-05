using System.Data;
using ClubService.Application.Api;
using ClubService.Application.Api.Exceptions;
using ClubService.Domain.Event.Member;
using ClubService.Domain.Model.Entity;
using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.Repository;
using ClubService.Domain.Repository.Transaction;

namespace ClubService.Application.Impl;

public class UpdateMemberService(
    IEventRepository eventRepository,
    IEventStoreTransactionManager eventStoreTransactionManager) : IUpdateMemberService
{
    public async Task<Guid> LockMember(Guid id)
    {
        var memberId = new MemberId(id);
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
    }
    
    public async Task<Guid> UnlockMember(Guid id)
    {
        var memberId = new MemberId(id);
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
    }
    
    public async Task<Guid> ChangeFullName(Guid id, string firstName, string lastName)
    {
        var memberId = id;
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
    }
    
    public async Task<Guid> ChangeEmail(Guid id, string email)
    {
        var memberId = id;
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
    }
}