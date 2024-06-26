using ClubService.Application.Api;
using ClubService.Application.Api.Exceptions;
using ClubService.Application.Commands;
using ClubService.Domain.Api;
using ClubService.Domain.Event;
using ClubService.Domain.Event.TennisClub;
using ClubService.Domain.Model.Entity;
using ClubService.Domain.Model.Enum;
using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.Repository;
using ClubService.Domain.Repository.Transaction;

namespace ClubService.Application.Impl;

public class RegisterAdminService(
    IEventRepository eventRepository,
    IAdminReadModelRepository adminReadModelRepository,
    ILoginRepository loginRepository,
    IPasswordHasherService passwordHasherService,
    ITransactionManager transactionManager,
    ILoggerService<RegisterAdminService> loggerService) : IRegisterAdminService
{
    public async Task<Guid> RegisterAdmin(AdminRegisterCommand adminRegisterCommand)
    {
        loggerService.LogRegisterAdmin(adminRegisterCommand.Username, adminRegisterCommand.FirstName,
            adminRegisterCommand.LastName, adminRegisterCommand.TennisClubId);

        var tennisClubId = new TennisClubId(adminRegisterCommand.TennisClubId);

        var tennisClubDomainEvents =
            await eventRepository.GetEventsForEntity<ITennisClubDomainEvent>(tennisClubId.Id, EntityType.TENNIS_CLUB);

        if (tennisClubDomainEvents.Count == 0)
        {
            loggerService.LogTennisClubNotFound(tennisClubId.Id);
            throw new TennisClubNotFoundException(tennisClubId.Id);
        }

        var tennisClub = new TennisClub();
        foreach (var domainEvent in tennisClubDomainEvents)
        {
            tennisClub.Apply(domainEvent);
        }

        switch (tennisClub.Status)
        {
            case TennisClubStatus.ACTIVE:
                var admins = await adminReadModelRepository.GetAdminsByTennisClubId(tennisClubId.Id);

                if (admins.Exists(admin => admin.Username == adminRegisterCommand.Username))
                {
                    loggerService.LogAdminUsernameAlreadyExists(
                        adminRegisterCommand.Username,
                        tennisClub.Name,
                        tennisClubId.Id);
                    throw new AdminUsernameAlreadyExistsException(
                        adminRegisterCommand.Username,
                        tennisClub.Name,
                        tennisClubId.Id
                    );
                }

                var admin = new Admin();
                var domainEvents = admin.ProcessAdminRegisterCommand(
                    adminRegisterCommand.Username,
                    new FullName(adminRegisterCommand.FirstName, adminRegisterCommand.LastName),
                    tennisClubId
                );
                var expectedEventCount = 0;

                await transactionManager.TransactionScope(async () =>
                {
                    foreach (var domainEvent in domainEvents)
                    {
                        admin.Apply(domainEvent);
                        expectedEventCount = await eventRepository.Append(domainEvent, expectedEventCount);
                    }

                    SaveLoginCredentials(admin.AdminId, adminRegisterCommand.Password);
                });

                loggerService.LogAdminRegistered(admin.AdminId.Id);
                return admin.AdminId.Id;
            case TennisClubStatus.LOCKED:
                throw new ConflictException("Tennis club is locked!");
            case TennisClubStatus.DELETED:
                throw new ConflictException("Tennis club already deleted!");
            default:
                throw new ArgumentOutOfRangeException(nameof(tennisClub.Status));
        }
    }

    private void SaveLoginCredentials(AdminId adminId, string password)
    {
        var userPassword = UserPassword.Create(adminId.Id, password, passwordHasherService);
        loginRepository.Add(userPassword);
    }
}