using ClubService.Application.Api;
using ClubService.Application.Api.Exceptions;
using ClubService.Application.Commands;
using ClubService.Domain.Model.Entity;
using ClubService.Domain.Model.Enum;
using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.Repository;
using ClubService.Domain.Repository.Transaction;

namespace ClubService.Application.Impl;

public class RegisterMemberService(
    IEventRepository eventRepository,
    IMemberReadModelRepository memberReadModelRepository,
    ITennisClubReadModelRepository tennisClubReadModelRepository,
    ISubscriptionTierReadModelRepository subscriptionTierReadModelRepository,
    IEventStoreTransactionManager eventStoreTransactionManager) : IRegisterMemberService
{
    public async Task<Guid> RegisterMember(MemberRegisterCommand memberRegisterCommand)
    {
        var tennisClubId = new TennisClubId(memberRegisterCommand.TennisClubId);
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
                
                var members = await memberReadModelRepository.GetMembersByTennisClubId(tennisClubId.Id);
                
                if (members.Any(member => member.Email == memberRegisterCommand.Email))
                {
                    throw new MemberEmailAlreadyExists(
                        memberRegisterCommand.Email,
                        tennisClubReadModel.Name,
                        tennisClubId.Id
                    );
                }
                
                var member = new Member();
                
                var domainEvents = member.ProcessMemberRegisterCommand(
                    memberRegisterCommand.FirstName,
                    memberRegisterCommand.LastName,
                    memberRegisterCommand.Email,
                    tennisClubId
                );
                var expectedEventCount = 0;
                
                await eventStoreTransactionManager.TransactionScope(async () =>
                {
                    foreach (var domainEvent in domainEvents)
                    {
                        member.Apply(domainEvent);
                        expectedEventCount = await eventRepository.Append(domainEvent, expectedEventCount);
                    }
                });
                
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