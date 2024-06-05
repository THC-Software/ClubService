namespace ClubService.Application.Commands;

public class TennisClubUpdateCommand(string? name, Guid? subscriptionTierId)
{
    public string? Name { get; } = name;
    public Guid? SubscriptionTierId { get; } = subscriptionTierId;
}