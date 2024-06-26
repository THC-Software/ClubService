using ClubService.Application.Api;
using ClubService.Application.Api.Exceptions;
using ClubService.Application.Dto;
using ClubService.Domain.Api;
using ClubService.Domain.Repository;

namespace ClubService.Application.Impl;

public class UserService(
    ILoginRepository loginRepository,
    IPasswordHasherService passwordHasherService) : IUserService
{
    public async Task ChangePassword(ChangePasswordDto changePasswordDto, string? jwtUserId)
    {
        if (jwtUserId == null || !jwtUserId.Equals(changePasswordDto.UserId.Id.ToString()))
        {
            throw new UnauthorizedAccessException("You do not have access to this resource.");
        }

        var userPassword = await loginRepository.GetById(changePasswordDto.UserId.Id);
        if (userPassword == null)
        {
            throw new UserNotFoundException($"User with id: {changePasswordDto.UserId} not found");
        }

        userPassword.ChangePassword(changePasswordDto.Password, passwordHasherService);
        await loginRepository.ChangePassword();
    }
}