using ClubService.Domain.Api;
using ClubService.Domain.Model.Entity;
using ClubService.Domain.Model.ValueObject;

namespace ClubService.Application.Dto;

public class ChangePasswordDto(Guid userId, string password)
{
    public UserId UserId { get; } = new(userId);
    public string Password { get; } = password;
}