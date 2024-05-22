using ClubService.Domain.Model.Enum;
using ClubService.Domain.Model.ValueObject;

namespace ClubService.Domain.Event.Admin;

public class AdminRegisteredEvent(
    AdminId adminId,
    string username,
    FullName name,
    TennisClubId tennisClubId,
    AdminStatus adminStatus) : IAdminDomainEvent
{
    public AdminId AdminId { get; } = adminId;
    public string Username { get; } = username;
    public FullName Name { get; } = name;
    public TennisClubId TennisClubId { get; } = tennisClubId;
    public AdminStatus AdminStatus { get; } = adminStatus;
}