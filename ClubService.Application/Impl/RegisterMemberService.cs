using ClubService.Application.Api;
using ClubService.Application.Api.Exceptions;
using ClubService.Application.Commands;
using ClubService.Domain.Event.SubscriptionTier;
using ClubService.Domain.Event.TennisClub;
using ClubService.Domain.Model.Entity;
using ClubService.Domain.Model.Enum;
using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.Repository;

namespace ClubService.Application.Impl;

public class RegisterMemberService(IEventRepository eventRepository) : IRegisterMemberService
{
    public async Task<string> RegisterMember(MemberRegisterCommand memberRegisterCommand)
    {
        var tennisClubId = new TennisClubId(new Guid(memberRegisterCommand.TennisClubId));
        var existingTennisClubDomainEvents = await eventRepository
            .GetEventsForEntity<ITennisClubDomainEvent>(tennisClubId.Id);
        
        if (existingTennisClubDomainEvents.Count == 0)
        {
            throw new TennisClubNotFoundException(tennisClubId.Id);
        }
        
        var tennisClub = new TennisClub();
        foreach (var domainEvent in existingTennisClubDomainEvents)
        {
            tennisClub.Apply(domainEvent);
        }
        
        if (tennisClub.Status.Equals(TennisClubStatus.LOCKED))
        {
            throw new ConflictException("Tennis club is locked!");
        }
        
        var subscriptionTierId = tennisClub.SubscriptionTierId;
        var existingSubscriptionTierDomainEvents =
            await eventRepository.GetEventsForEntity<ISubscriptionTierDomainEvent>(subscriptionTierId.Id);
        
        if (existingSubscriptionTierDomainEvents.Count == 0)
        {
            throw new SubscriptionTierNotFoundException(subscriptionTierId.Id);
        }
        
        var member = new Member();
        
        var memberDomainEvents = member.ProcessMemberRegisterCommand(
            memberRegisterCommand.FirstName,
            memberRegisterCommand.LastName,
            memberRegisterCommand.Email,
            memberRegisterCommand.TennisClubId
        );
        
        foreach (var memberDomainEvent in memberDomainEvents)
        {
            member.Apply(memberDomainEvent);
            await eventRepository.Append(memberDomainEvent);
        }
        
        return member.MemberId.Id.ToString();
    }
}