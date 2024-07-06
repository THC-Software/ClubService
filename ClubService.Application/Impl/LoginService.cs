using ClubService.Application.Api;
using ClubService.Application.Api.Exceptions;
using ClubService.Application.Commands;
using ClubService.Application.Dto;
using ClubService.Application.Dto.Enums;
using ClubService.Application.EventHandlers;
using ClubService.Domain.Api;
using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.Repository;

namespace ClubService.Application.Impl;

public class LoginService(
    IPasswordHasherService passwordHasherService,
    IAdminReadModelRepository adminReadModelRepository,
    IMemberReadModelRepository memberReadModelRepository,
    ISystemOperatorReadModelRepository systemOperatorReadModelRepository,
    ILoginRepository loginRepository,
    ILoggerService<LoginService> loggerService) : ILoginService
{
    public async Task<UserInformationDto> Login(LoginCommand loginCommand)
    {
        // TODO: Prevent dereferencing null
        //loggerService.LogLogin(loginCommand.Username, loginCommand.TennisClubId.Id);

        if (loginCommand.TennisClubId == null)
        {
            // System Operator login
            var systemOperator =
                await systemOperatorReadModelRepository.GetSystemOperatorByUsername(loginCommand.Username);

            if (systemOperator != null)
            {
                var userPassword = await loginRepository.GetById(systemOperator.SystemOperatorId.Id);
                if (userPassword == null)
                {
                    loggerService.LogUserNotFound(systemOperator.SystemOperatorId.Id);
                    throw new UserNotFoundException("User not present in Login Database");
                }

                if (!passwordHasherService.VerifyPassword(userPassword.HashedPassword, loginCommand.Password))
                {
                    // again we are just allowed to throw this here as the user will not come in contact with it.
                    loggerService.LogLoginFailed(systemOperator.SystemOperatorId.Id);
                    throw new WrongPasswordException();
                }

                loggerService.LogUserLoggedIn(systemOperator.SystemOperatorId.Id, systemOperator.Username,
                    UserRole.SYSTEM_OPERATOR.ToString(),
                    string.Empty);
                return new UserInformationDto(new UserId(systemOperator.SystemOperatorId.Id), systemOperator.Username,
                    UserRole.SYSTEM_OPERATOR,
                    UserStatus.ACTIVE, null);
            }

            // We are only allowed to return this exception here in our backend as this function is only called by the gateway
            // and the user will never see this exception. In the gateway we can't return the same exception as we do not want
            // the user to know if the username was wrong or just the password.
            loggerService.LogUserNotFound(loginCommand.Username);
            throw new UserNotFoundException(loginCommand.Username);
        }

        // Admin and Member login
        var admin = await adminReadModelRepository.GetAdminByTennisClubIdAndUsername(loginCommand.TennisClubId.Id,
            loginCommand.Username);
        var member = await
            memberReadModelRepository.GetMemberByTennisClubIdAndUsername(loginCommand.TennisClubId.Id,
                loginCommand.Username);

        if (admin != null)
        {
            var userPassword = await loginRepository.GetById(admin.AdminId.Id);
            if (userPassword == null)
            {
                loggerService.LogUserNotFound(admin.AdminId.Id);
                throw new UserNotFoundException("User not present in Login Database");
            }

            if (!passwordHasherService.VerifyPassword(userPassword.HashedPassword, loginCommand.Password))
            {
                // again we are just allowed to throw this here as the user will not come in contact with it.
                loggerService.LogLoginFailed(admin.AdminId.Id);
                throw new WrongPasswordException();
            }

            loggerService.LogUserLoggedIn(admin.AdminId.Id, admin.Username, UserRole.ADMIN.ToString(),
                admin.Status.ToString());
            return new UserInformationDto(new UserId(admin.AdminId.Id), admin.Username, UserRole.ADMIN, admin.Status
                .ToUserStatus(), loginCommand.TennisClubId);
        }

        if (member != null)
        {
            var userPassword = await loginRepository.GetById(member.MemberId.Id);

            if (userPassword == null)
            {
                loggerService.LogUserNotFound(member.MemberId.Id);
                throw new UserNotFoundException("User not present in Login Database");
            }

            if (!passwordHasherService.VerifyPassword(userPassword.HashedPassword, loginCommand.Password))
            {
                // again we are just allowed to throw this here as the user will not come in contact with it.
                loggerService.LogLoginFailed(member.MemberId.Id);
                throw new WrongPasswordException();
            }

            loggerService.LogUserLoggedIn(member.MemberId.Id, member.Email, UserRole.MEMBER.ToString(),
                member.Status.ToString());
            return new UserInformationDto(new UserId(member.MemberId.Id), member.Email, UserRole.MEMBER,
                member.Status.ToUserStatus(), loginCommand.TennisClubId);
        }

        // We are only allowed to return this exception here in our backend as this function is only called by the gateway
        // and the user will never see this exception. In the gateway we can't return the same exception as we do not want
        // the user to know if the username was wrong or just the password.
        loggerService.LogUserNotFound(loginCommand.Username);
        throw new UserNotFoundException(loginCommand.TennisClubId.Id, loginCommand.Username);
    }
}