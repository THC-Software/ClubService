namespace ClubService.Application.Commands;

public class TennisClubRegisterCommand(string name, string subscriptionTierId)
{
    public string Name { get; } = name;
    public string SubscriptionTierId { get; } = subscriptionTierId;
}