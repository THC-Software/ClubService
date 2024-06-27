using System.Security.Authentication;
using ClubService.Application.Api;
using ClubService.Application.Api.Exceptions;
using ClubService.Domain.Event;
using ClubService.Domain.Event.Member;
using ClubService.Domain.Event.TennisClub;
using ClubService.Domain.Model.Entity;
using ClubService.Domain.Model.Enum;
using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.Repository;
using ClubService.Domain.Repository.Transaction;

namespace ClubService.Application.Impl;

public class DeleteMemberService(
    IEventRepository eventRepository,
    IEventStoreTransactionManager eventStoreTransactionManager,
    ILoggerService<DeleteMemberService> loggerService) : IDeleteMemberService
{
    public async Task<Guid> DeleteMember(
        Guid id,
        string? jwtTennisClubId)
    {
        if (jwtTennisClubId == null)
        {
            throw new AuthenticationException("Authentication error.");
        }

        loggerService.LogDeleteMember(id);

        var memberId = new MemberId(id);
        var existingMemberDomainEvents =
            await eventRepository.GetEventsForEntity<IMemberDomainEvent>(memberId.Id, EntityType.MEMBER);

        if (existingMemberDomainEvents.Count == 0)
        {
            loggerService.LogMemberNotFound(id);
            throw new MemberNotFoundException(memberId.Id);
        }

        var member = new Member();
        foreach (var domainEvent in existingMemberDomainEvents)
        {
            member.Apply(domainEvent);
        }
        
        if (!jwtTennisClubId.Equals(member.TennisClubId.Id.ToString()))
        {
            throw new UnauthorizedAccessException("You do not have access to this resource.");
        }

        var tennisClubDomainEvents =
            await eventRepository.GetEventsForEntity<ITennisClubDomainEvent>(member.TennisClubId.Id,
                EntityType.TENNIS_CLUB);

        if (tennisClubDomainEvents.Count == 0)
        {
            loggerService.LogTennisClubNotFound(member.TennisClubId.Id);
            throw new TennisClubNotFoundException(member.TennisClubId.Id);
        }

        var tennisClub = new TennisClub();
        foreach (var domainEvent in tennisClubDomainEvents)
        {
            tennisClub.Apply(domainEvent);
        }

        switch (tennisClub.Status)
        {
            case TennisClubStatus.ACTIVE:
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
                    loggerService.LogInvalidOperationException(ex);
                    throw new ConflictException(ex.Message, ex);
                }

                loggerService.LogMemberDeleted(id);
                return id;
            case TennisClubStatus.LOCKED:
                throw new ConflictException("Tennis club is locked!");
            case TennisClubStatus.DELETED:
                throw new ConflictException("Tennis club already deleted!");
            default:
                throw new ArgumentOutOfRangeException(nameof(tennisClub.Status));
        }
    }
}