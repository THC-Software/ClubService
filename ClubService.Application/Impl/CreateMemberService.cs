using ClubService.Application.Api;
using ClubService.Application.Api.Exceptions;
using ClubService.Application.Commands;
using ClubService.Domain.Event.SubscriptionTier;
using ClubService.Domain.Event.TennisClub;
using ClubService.Domain.Model.Entity;
using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.Repository;

namespace ClubService.Application.Impl;

public class CreateMemberService(IEventRepository eventRepository) : ICreateMemberService
{
    public async Task<string> CreateMember(MemberCreateCommand memberCreateCommand)
    {
        var tennisClubId = new TennisClubId(new Guid(memberCreateCommand.TennisClubId));
        var existingTennisClubDomainEvents = eventRepository
            .GetEventsForEntity<ITennisClubDomainEvent>(tennisClubId.Id)
            .OrderBy(e => e.Timestamp)
            .ToList();
        
        if (existingTennisClubDomainEvents.Count == 0)
        {
            throw new TennisClubNotFoundException("No events found!");
        }
        
        var tennisClub = TennisClub.Create(tennisClubId);
        foreach (var domainEvent in existingTennisClubDomainEvents)
        {
            tennisClub.Apply(domainEvent);
        }
        
        if (tennisClub.IsLocked)
        {
            throw new ConflictException("Tennis club is locked!");
        }
        
        var subscriptionTierId = tennisClub.SubscriptionTierId;
        var existingSubscriptionTierDomainEvents =
            eventRepository.GetEventsForEntity<ISubscriptionTierDomainEvent>(subscriptionTierId.Id);
        
        if (existingSubscriptionTierDomainEvents.Count == 0)
        {
            throw new SubscriptionTierNotFoundException($"Subscription Tier '{subscriptionTierId}' not found!");
        }
        
        var subscriptionTier = new SubscriptionTier();
        foreach (var domainEvent in existingSubscriptionTierDomainEvents)
        {
            subscriptionTier.Apply(domainEvent);
        }
        
        if (tennisClub.MemberIds.Count >= subscriptionTier.MaxMemberCount)
        {
            // TODO: should we remove the TennisClubMemberLimitExceededEvent?
            throw new ArgumentException("Member limit exceeded!");
        }
        
        var member = new Member();
        
        var memberDomainEvents = member.ProcessMemberCreatedCommand(
            memberCreateCommand.FirstName,
            memberCreateCommand.LastName,
            memberCreateCommand.Email,
            memberCreateCommand.TennisClubId
        );
        
        foreach (var memberDomainEvent in memberDomainEvents)
        {
            member.Apply(memberDomainEvent);
            await eventRepository.Save(memberDomainEvent);
        }
        
        return member.MemberId.Id.ToString();
    }
}