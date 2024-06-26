using ClubService.Application.Api;
using ClubService.Application.Api.Exceptions;
using ClubService.Application.Commands;
using ClubService.Domain.Api;
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
    ITransactionManager transactionManager,
    ILoginRepository loginRepository,
    IPasswordHasherService passwordHasherService,
    ILoggerService<RegisterMemberService> loggerService) : IRegisterMemberService
{
    public async Task<Guid> RegisterMember(MemberRegisterCommand memberRegisterCommand)
    {
        loggerService.LogRegisterMember(memberRegisterCommand.FirstName,
            memberRegisterCommand.LastName, memberRegisterCommand.Email, memberRegisterCommand.TennisClubId);

        var tennisClubId = new TennisClubId(memberRegisterCommand.TennisClubId);
        var tennisClubReadModel = await tennisClubReadModelRepository.GetTennisClubById(tennisClubId.Id);

        if (tennisClubReadModel == null)
        {
            loggerService.LogTennisClubNotFound(tennisClubId.Id);
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
                    loggerService.LogSubscriptionTierNotFound(tennisClubReadModel.SubscriptionTierId.Id);
                    throw new SubscriptionTierNotFoundException(tennisClubReadModel.SubscriptionTierId.Id);
                }

                if (tennisClubReadModel.MemberCount + 1 > subscriptionTierReadModel.MaxMemberCount)
                {
                    loggerService.LogMemberLimitExceeded(tennisClubId.Id, subscriptionTierReadModel.MaxMemberCount);
                    throw new MemberLimitExceededException(subscriptionTierReadModel.MaxMemberCount);
                }

                var members = await memberReadModelRepository.GetMembersByTennisClubId(tennisClubId.Id);

                if (members.Exists(member => member.Email == memberRegisterCommand.Email))
                {
                    loggerService.LogMemberEmailAlreadyExists(
                        memberRegisterCommand.Email,
                        tennisClubReadModel.Name,
                        tennisClubId.Id);
                    throw new MemberEmailAlreadyExistsException(
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

                await transactionManager.TransactionScope(async () =>
                {
                    foreach (var domainEvent in domainEvents)
                    {
                        member.Apply(domainEvent);
                        expectedEventCount = await eventRepository.Append(domainEvent, expectedEventCount);
                    }

                    SaveLoginCredentials(member.MemberId, memberRegisterCommand.Password);
                });

                loggerService.LogMemberRegistered(member.MemberId.Id);
                return member.MemberId.Id;
            case TennisClubStatus.LOCKED:
                throw new ConflictException("Tennis club is locked!");
            case TennisClubStatus.DELETED:
                throw new ConflictException("Tennis club already deleted!");
            default:
                throw new ArgumentOutOfRangeException(nameof(tennisClubReadModel.Status));
        }
    }

    private void SaveLoginCredentials(MemberId memberId, string password)
    {
        var userPassword = UserPassword.Create(memberId.Id, password, passwordHasherService);
        loginRepository.Add(userPassword);
    }
}