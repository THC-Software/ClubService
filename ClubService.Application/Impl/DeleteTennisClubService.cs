using ClubService.Application.Api;
using ClubService.Application.Api.Exceptions;
using ClubService.Domain.Event;
using ClubService.Domain.Event.TennisClub;
using ClubService.Domain.Model.Entity;
using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.Repository;
using ClubService.Domain.Repository.Transaction;

namespace ClubService.Application.Impl;

public class DeleteTennisClubService(
    IEventRepository eventRepository,
    IMemberReadModelRepository memberReadModelRepository,
    IAdminReadModelRepository adminReadModelRepository,
    IDeleteMemberService deleteMemberService,
    IDeleteAdminService deleteAdminService,
    ITransactionManager transactionManager,
    ILoggerService<DeleteTennisClubService> loggerService) : IDeleteTennisClubService
{
    public async Task<Guid> DeleteTennisClub(Guid id, string? jwtRole, string? jwtTennisClubId)
    {
        loggerService.LogDeleteTennisClub(id);

        if (jwtRole is not "SYSTEM_OPERATOR" && (jwtTennisClubId == null ||
                                                 !jwtTennisClubId.Equals(id.ToString())))
        {
            throw new UnauthorizedAccessException("You do not have access to this resource.");
        }

        var tennisClubId = new TennisClubId(id);

        var existingTennisClubDomainEvents =
            await eventRepository.GetEventsForEntity<ITennisClubDomainEvent>(tennisClubId.Id, EntityType.TENNIS_CLUB);

        if (existingTennisClubDomainEvents.Count == 0)
        {
            loggerService.LogTennisClubNotFound(id);
            throw new TennisClubNotFoundException(tennisClubId.Id);
        }

        var membersForTennisClub = await memberReadModelRepository.GetMembersByTennisClubId(tennisClubId.Id);
        var adminsForTennisClub = await adminReadModelRepository.GetAdminsByTennisClubId(tennisClubId.Id);

        try
        {
            await transactionManager.TransactionScope(async () =>
            {
                foreach (var member in membersForTennisClub)
                {
                    await deleteMemberService.DeleteMember(member.MemberId.Id, jwtTennisClubId);
                }

                foreach (var admin in adminsForTennisClub)
                {
                    // Pass empty jwtUserId as DeleteAdmin itself checks that an admin does not delete its own account
                    // With deleting the tennis club the admin deletes its own account which is ok in this case
                    await deleteAdminService.DeleteAdmin(admin.AdminId.Id, "", tennisClubId.Id.ToString());
                }

                var tennisClub = new TennisClub();
                foreach (var domainEvent in existingTennisClubDomainEvents)
                {
                    tennisClub.Apply(domainEvent);
                }

                var domainEvents = tennisClub.ProcessTennisClubDeleteCommand();
                var expectedEventCount = existingTennisClubDomainEvents.Count;

                foreach (var domainEvent in domainEvents)
                {
                    tennisClub.Apply(domainEvent);
                    expectedEventCount = await eventRepository.Append(domainEvent, expectedEventCount);
                }
            });
        }
        catch (InvalidOperationException ex)
        {
            loggerService.LogInvalidOperationException(ex);
            throw new ConflictException(ex.Message, ex);
        }

        loggerService.LogTennisClubDeleted(id);
        return id;
    }
}