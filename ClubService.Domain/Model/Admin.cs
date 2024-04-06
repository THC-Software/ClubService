namespace ClubService.Domain.Model;

public class Admin
{
    public AdminId Id { get; private set; }
    public string Username { get; private set; }
    public string Name { get; private set; }
    public TennisClubId TennisClubId { get; private set; }

    private Admin() { }

    private Admin(AdminId id, string username, string name, TennisClubId tennisClubId)
    {
        Id = id;
        Username = username;
        Name = name;
        TennisClubId = tennisClubId;
    }

    public static Admin Create(AdminId id, string username, string name, TennisClubId tennisClubId)
    {
        return new Admin(id, username, name, tennisClubId);
    }
}