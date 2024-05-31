using ClubService.Domain.Event.Admin;
using ClubService.Domain.Model.Enum;
using ClubService.Domain.Model.ValueObject;

namespace ClubService.Domain.ReadModel;

public class AdminReadModel
{
    public AdminId AdminId { get; }
    public string Username { get; }
    public FullName Name { get; }
    public TennisClubId TennisClubId { get; }
    public AdminStatus Status { get; }
    
    private AdminReadModel()
    {
    } // needed by efcore
    
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