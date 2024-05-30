﻿using ClubService.Application.Api;
using ClubService.Application.Api.Exceptions;
using ClubService.Domain.Event.Member;
using ClubService.Domain.Model.Entity;
using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.Repository;

namespace ClubService.Application.Impl;

public class DeleteMemberService(IEventRepository eventRepository) : IDeleteMemberService
{
    public async Task<string> DeleteMember(string id)
    {
        var memberId = new MemberId(new Guid(id));
        var existingMemberDomainEvents = await eventRepository.GetEventsForEntity<IMemberDomainEvent>(memberId.Id);
        
        if (existingMemberDomainEvents.Count == 0)
        {
            throw new MemberNotFoundException(memberId.Id);
        }
        
        var initialEventCount = existingMemberDomainEvents.Count;
        
        var member = new Member();
        foreach (var domainEvent in existingMemberDomainEvents)
        {
            member.Apply(domainEvent);
        }
        
        try
        {
            var domainEvents = member.ProcessMemberDeleteCommand();
            
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
                    "Additional events added during processing of delete member!");
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