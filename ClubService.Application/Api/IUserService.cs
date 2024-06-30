using ClubService.Application.Dto;

namespace ClubService.Application.Api;

public interface IUserService
{
    Task ChangePassword(ChangePasswordDto changePasswordDto, string? jwtUserId);
}