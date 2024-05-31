using ClubService.Domain.Event.Admin;
using ClubService.Domain.Model.Enum;
using ClubService.Domain.Model.ValueObject;

namespace ClubService.Domain.ReadModel;

public class AdminReadModel
{
    public AdminId AdminId { get; } = null!;
    public string Username { get; } = null!;
    public FullName Name { get; } = null!;
    public TennisClubId TennisClubId { get; } = null!;
    public AdminStatus Status { get; }
    
    // needed by efcore
    private AdminReadModel()
    {
    }
    
    private AdminReadModel(
        AdminId adminId,
        string username,
        FullName name,
        TennisClubId tennisClubId,
        AdminStatus status)
    {
        AdminId = adminId;
        Username = username;
        Name = name;
        TennisClubId = tennisClubId;
        Status = status;
    }
    
    public static AdminReadModel FromDomainEvent(AdminRegisteredEvent adminRegisteredEvent)
    {
        return new AdminReadModel(
            adminRegisteredEvent.AdminId,
            adminRegisteredEvent.Username,
            adminRegisteredEvent.Name,
            adminRegisteredEvent.TennisClubId,
            adminRegisteredEvent.Status
        );
    }
}