using ClubService.Application.Api;
using ClubService.Application.Api.Exceptions;
using ClubService.Application.Commands;
using ClubService.Domain.Api;
using ClubService.Domain.Repository;

namespace ClubService.Application.Impl;

public class UserService(
    ILoginRepository loginRepository,
    IPasswordHasherService passwordHasherService) : IUserService
{
    public async Task ChangePassword(ChangePasswordCommand changePasswordCommand, string? jwtUserId)
    {
        if (jwtUserId == null || !jwtUserId.Equals(changePasswordCommand.UserId.Id.ToString()))
        {
            throw new UnauthorizedAccessException("You do not have access to this resource.");
        }

        var userPassword = await loginRepository.GetById(changePasswordCommand.UserId.Id);
        if (userPassword == null)
        {
            throw new UserNotFoundException($"User with id: {changePasswordCommand.UserId} not found");
        }

        userPassword.ChangePassword(changePasswordCommand.Password, passwordHasherService);
        await loginRepository.ChangePassword();
    }
}