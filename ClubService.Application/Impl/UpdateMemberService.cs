using ClubService.Application.Api;
using ClubService.Application.Api.Exceptions;
using ClubService.Domain.Event.Member;
using ClubService.Domain.Model.Entity;
using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.Repository;

namespace ClubService.Application.Impl;

public class UpdateMemberService(IEventRepository eventRepository) : IUpdateMemberService
{
    public async Task<string> LockMember(string id)
    {
        var memberId = new MemberId(new Guid(id));
        var existingMemberDomainEvents = await eventRepository.GetEventsForEntity<IMemberDomainEvent>(memberId.Id);
        
        if (existingMemberDomainEvents.Count == 0)
        {
            throw new MemberNotFoundException("No member events found!");
        }
        
        var initialEventCount = existingMemberDomainEvents.Count;
        
        var member = new Member();
        foreach (var domainEvent in existingMemberDomainEvents)
        {
            member.Apply(domainEvent);
        }
        
        try
        {
            var memberLockDomainEvents = member.ProcessMemberLockCommand();
            
            await eventRepository.BeginTransactionAsync();
            
            foreach (var domainEvent in memberLockDomainEvents)
            {
                member.Apply(domainEvent);
                await eventRepository.Append(domainEvent);
            }
            
            existingMemberDomainEvents = await eventRepository.GetEventsForEntity<IMemberDomainEvent>(memberId.Id);
            
            if (existingMemberDomainEvents.Count != initialEventCount + memberLockDomainEvents.Count)
            {
                throw new ConcurrencyException(
                    "Additional events added during processing locking the member!");
            }
            
            await eventRepository.CommitTransactionAsync();
        }
        catch (InvalidOperationException ex)
        {
            throw new ConflictException(ex.Message, ex);
        }
        catch (ConcurrencyException)
        {
            await eventRepository.RollbackTransactionAsync();
            throw;
        }
        
        return id;
    }
    
    public async Task<string> UnlockMember(string id)
    {
        var memberId = new MemberId(new Guid(id));
        var existingMemberDomainEvents = await eventRepository.GetEventsForEntity<IMemberDomainEvent>(memberId.Id);
        
        if (existingMemberDomainEvents.Count == 0)
        {
            throw new MemberNotFoundException("No member events found!");
        }
        
        var initialEventCount = existingMemberDomainEvents.Count;
        
        var member = new Member();
        foreach (var domainEvent in existingMemberDomainEvents)
        {
            member.Apply(domainEvent);
        }
        
        try
        {
            var domainEvents = member.ProcessMemberUnlockCommand();
            
            await eventRepository.BeginTransactionAsync();
            
            foreach (var domainEvent in domainEvents)
            {
                member.Apply(domainEvent);
                await eventRepository.Append(domainEvent);
            }
            
            existingMemberDomainEvents = await eventRepository.GetEventsForEntity<IMemberDomainEvent>(memberId.Id);
            
            if (existingMemberDomainEvents.Count != initialEventCount + domainEvents.Count)
            {
                throw new ConcurrencyException(
                    "Additional events added during processing unlocking the member!");
            }
            
            await eventRepository.CommitTransactionAsync();
        }
        catch (InvalidOperationException ex)
        {
            throw new ConflictException(ex.Message, ex);
        }
        catch (ConcurrencyException)
        {
            await eventRepository.RollbackTransactionAsync();
            throw;
        }
        
        return id;
    }
}