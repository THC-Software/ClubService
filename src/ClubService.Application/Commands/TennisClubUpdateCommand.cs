namespace ClubService.Application.Commands;

public class TennisClubUpdateCommand(string name, string subscriptionTier)
{
    public string Name { get; } = name;
    public string SubscriptionTier { get; } = subscriptionTier;
}