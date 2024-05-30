﻿using System.Data;
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
}