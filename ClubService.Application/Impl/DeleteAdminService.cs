using ClubService.Application.Api;
using ClubService.Application.Api.Exceptions;
using ClubService.Domain.Event;
using ClubService.Domain.Event.Admin;
using ClubService.Domain.Model.Entity;
using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.Repository;
using ClubService.Domain.Repository.Transaction;

namespace ClubService.Application.Impl;

public class DeleteAdminService(
    IEventRepository eventRepository,
    ITransactionManager transactionManager,
    ILoggerService<DeleteAdminService> loggerService) : IDeleteAdminService
{
    public async Task<Guid> DeleteAdmin(Guid id, string? jwtUserId, string? jwtTennisClubId)
    {
        loggerService.LogDeleteAdmin(id);

        if (jwtUserId == null || jwtTennisClubId == null)
        {
            throw new UnauthorizedAccessException("You do not have access to this resource.");
        }

        // An admin is not allowed to delete its own account to prevent that the admin is deleted
        // while it is the only admin for the tennis club.
        // Instead the tennis club needs to be deleted to also delete the only existing admin account for a tennis club.
        if (jwtUserId.Equals(id.ToString()))
        {
            throw new InvalidOperationException(
                "You can't delete your own account. Please delete the tennis club instead.");
        }

        var adminId = new AdminId(id);
        var existingAdminDomainEvents =
            await eventRepository.GetEventsForEntity<IAdminDomainEvent>(adminId.Id, EntityType.ADMIN);

        if (existingAdminDomainEvents.Count == 0)
        {
            loggerService.LogAdminNotFound(id);
            throw new AdminNotFoundException(adminId.Id);
        }

        var admin = new Admin();
        foreach (var domainEvent in existingAdminDomainEvents)
        {
            admin.Apply(domainEvent);
        }

        if (!jwtTennisClubId.Equals(admin.TennisClubId.Id.ToString()))
        {
            throw new UnauthorizedAccessException("You do not have access to this resource.");
        }

        try
        {
            var domainEvents = admin.ProcessAdminDeleteCommand();
            var expectedEventCount = existingAdminDomainEvents.Count;

            await transactionManager.TransactionScope(async () =>
            {
                foreach (var domainEvent in domainEvents)
                {
                    admin.Apply(domainEvent);
                    expectedEventCount = await eventRepository.Append(domainEvent, expectedEventCount);
                }
            });
        }
        catch (InvalidOperationException ex)
        {
            loggerService.LogInvalidOperationException(ex);
            throw new ConflictException(ex.Message, ex);
        }

        loggerService.LogAdminDeleted(id);
        return id;
    }
}