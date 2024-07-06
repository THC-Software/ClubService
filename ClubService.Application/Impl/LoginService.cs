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
        if (loginCommand.TennisClubId == null)
        {
            loggerService.LogLogin(loginCommand.Username);
            return await LoginSystemOperator(loginCommand);
        }

        loggerService.LogLogin(loginCommand.Username, loginCommand.TennisClubId.Id);
        return await LoginAdminOrMember(loginCommand);
    }

    private async Task<UserInformationDto> LoginSystemOperator(LoginCommand loginCommand)
    {
        var systemOperator =
            await systemOperatorReadModelRepository.GetSystemOperatorByUsername(loginCommand.Username);

        if (systemOperator != null)
        {
            return await LoginUser(systemOperator.SystemOperatorId.Id, loginCommand.Username, loginCommand.Password,
                UserRole.SYSTEM_OPERATOR, UserStatus.ACTIVE, null);
        }

        loggerService.LogUserNotFound(loginCommand.Username);
        throw new UserNotFoundException(loginCommand.Username);
    }

    private async Task<UserInformationDto> LoginAdminOrMember(LoginCommand loginCommand)
    {
        var admin = await adminReadModelRepository.GetAdminByTennisClubIdAndUsername(loginCommand.TennisClubId!.Id,
            loginCommand.Username);
        if (admin != null)
        {
            return await LoginUser(admin.AdminId.Id, loginCommand.Username, loginCommand.Password, UserRole.ADMIN,
                admin.Status.ToUserStatus(), admin.TennisClubId);
        }

        var member =
            await memberReadModelRepository.GetMemberByTennisClubIdAndUsername(loginCommand.TennisClubId.Id,
                loginCommand.Username);
        if (member != null)
        {
            return await LoginUser(member.MemberId.Id, loginCommand.Username, loginCommand.Password, UserRole.MEMBER,
                member.Status.ToUserStatus(), member.TennisClubId);
        }

        loggerService.LogUserNotFound(loginCommand.Username);
        throw new UserNotFoundException(loginCommand.TennisClubId.Id, loginCommand.Username);
    }

    private async Task<UserInformationDto> LoginUser(
        Guid userId,
        string username,
        string password,
        UserRole role,
        UserStatus userStatus,
        TennisClubId? tennisClubId)
    {
        var userPassword = await loginRepository.GetById(userId);
        if (userPassword == null)
        {
            loggerService.LogUserNotFound(userId);
            throw new UserNotFoundException("User not present in Login Database");
        }

        if (!passwordHasherService.VerifyPassword(userPassword.HashedPassword, password))
        {
            // again we are just allowed to throw this here as the user will not come in contact with it.
            loggerService.LogLoginFailed(userId);
            throw new WrongPasswordException();
        }

        loggerService.LogUserLoggedIn(userId, username, role.ToString(), userStatus.ToString());
        return new UserInformationDto(new UserId(userId), username, role, userStatus, tennisClubId);
    }
}