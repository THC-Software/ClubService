namespace ClubService.Application.Dto;

public class SubscriptionTierDto(string subscriptionTierId, string name, int maxMemberCount)
{
    public string SubscriptionTierId { get; } = subscriptionTierId;
    public string Name { get; } = name;
    public int MaxMemberCount { get; } = maxMemberCount;
}