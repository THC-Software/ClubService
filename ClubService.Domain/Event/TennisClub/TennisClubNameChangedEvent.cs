namespace ClubService.Domain.Event.TennisClub;

public class TennisClubNameChangedEvent(string name) : ITennisClubDomainEvent
{
    public string Name { get; } = name;
}