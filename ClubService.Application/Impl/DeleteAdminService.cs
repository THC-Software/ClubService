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
    IEventStoreTransactionManager eventStoreTransactionManager,
    ILoggerService<DeleteAdminService> loggerService) : IDeleteAdminService
{
    public async Task<Guid> DeleteAdmin(Guid id)
    {
        loggerService.LogDeleteAdmin(id);

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

        try
        {
            var domainEvents = admin.ProcessAdminDeleteCommand();
            var expectedEventCount = existingAdminDomainEvents.Count;

            await eventStoreTransactionManager.TransactionScope(async () =>
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