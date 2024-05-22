namespace ClubService.Application.Commands;

public class TennisClubUpdateCommand(string? name, string? subscriptionTierId)
{
    public string? Name { get; } = name;
    public string? SubscriptionTierId { get; } = subscriptionTierId;
}