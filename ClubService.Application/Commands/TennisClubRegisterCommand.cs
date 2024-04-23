namespace ClubService.Application.Commands;

public class TennisClubRegisterCommand(string name, string subscriptionTier)
{
    public string Name { get; } = name;
    public string SubscriptionTier { get; } = subscriptionTier;
}