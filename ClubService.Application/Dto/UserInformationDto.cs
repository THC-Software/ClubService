using ClubService.Application.Dto.Enums;
using ClubService.Domain.Model.ValueObject;

namespace ClubService.Application.Dto;

public class UserInformationDto(
    UserId userId,
    string username,
    UserRole userRole,
    UserStatus userStatus,
    TennisClubId tennisClubId)
{
    public UserId UserId { get; } = userId;
    public string Username { get; } = username;
    public UserRole UserRole { get; } = userRole;
    public UserStatus UserStatus { get; } = userStatus;
    public TennisClubId TennisClubId { get; } = tennisClubId;
}