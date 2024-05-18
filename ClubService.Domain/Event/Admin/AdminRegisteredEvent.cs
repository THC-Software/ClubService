using ClubService.Domain.Model.ValueObject;

namespace ClubService.Domain.Event.Admin;

public class AdminRegisteredEvent(
    AdminId adminId,
    string username,
    FullName name,
    TennisClubId tennisClubId,
    bool isDeleted) : IAdminDomainEvent
{
    public AdminId AdminId { get; } = adminId;
    public string Username { get; } = username;
    public FullName Name { get; } = name;
    public TennisClubId TennisClubId { get; } = tennisClubId;
    public bool IsDeleted { get; } = isDeleted;
}