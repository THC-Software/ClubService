using ClubService.Application.Api;
using ClubService.Application.Api.Exceptions;
using ClubService.Application.Commands;
using ClubService.Domain.Model.Entity;
using ClubService.Domain.Model.Enum;
using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.Repository;

namespace ClubService.Application.Impl;

public class RegisterMemberService(
    IEventRepository eventRepository,
    ITennisClubReadModelRepository tennisClubReadModelRepository,
    ISubscriptionTierReadModelRepository subscriptionTierReadModelRepository) : IRegisterMemberService
{
    public async Task<Guid> RegisterMember(MemberRegisterCommand memberRegisterCommand)
    {
        var tennisClubId = new TennisClubId(new Guid(memberRegisterCommand.TennisClubId));
        var tennisClubReadModel = await tennisClubReadModelRepository.GetTennisClubById(tennisClubId.Id);
        
        if (tennisClubReadModel == null)
        {
            throw new TennisClubNotFoundException(tennisClubId.Id);
        }
        
        switch (tennisClubReadModel.Status)
        {
            case TennisClubStatus.ACTIVE:
                var subscriptionTierReadModel = await subscriptionTierReadModelRepository.GetSubscriptionTierById(
                    tennisClubReadModel.SubscriptionTierId.Id
                );
                
                if (subscriptionTierReadModel == null)
                {
                    throw new SubscriptionTierNotFoundException(tennisClubReadModel.SubscriptionTierId.Id);
                }
                
                if (tennisClubReadModel.MemberCount + 1 > subscriptionTierReadModel.MaxMemberCount)
                {
                    throw new MemberLimitExceededException(subscriptionTierReadModel.MaxMemberCount);
                }
                
                var member = new Member();
                
                var domainEvents = member.ProcessMemberRegisterCommand(
                    memberRegisterCommand.FirstName,
                    memberRegisterCommand.LastName,
                    memberRegisterCommand.Email,
                    memberRegisterCommand.TennisClubId
                );
                var expectedEventCount = 0;
                
                foreach (var domainEvent in domainEvents)
                {
                    member.Apply(domainEvent);
                    expectedEventCount = await eventRepository.Append(domainEvent, expectedEventCount);
                }
                
                return member.MemberId.Id;
            case TennisClubStatus.LOCKED:
                throw new ConflictException("Tennis club is locked!");
            case TennisClubStatus.DELETED:
                throw new ConflictException("Tennis club already deleted!");
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}