﻿using System.Data;
using ClubService.Application.Api;
using ClubService.Application.Api.Exceptions;
using ClubService.Domain.Event.Member;
using ClubService.Domain.Model.Entity;
using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.Repository;
using ClubService.Domain.Repository.Transaction;

namespace ClubService.Application.Impl;

public class DeleteMemberService(
    IEventRepository eventRepository,
    IEventStoreTransactionManager eventStoreTransactionManager) : IDeleteMemberService
{
    public async Task<Guid> DeleteMember(Guid id)
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
            var domainEvents = member.ProcessMemberDeleteCommand();
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
}