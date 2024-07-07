using ClubService.Application.Commands;

namespace ClubService.Application.Api;

public interface IUserService
{
    Task ChangePassword(ChangePasswordCommand changePasswordCommand, string? jwtUserId);
}