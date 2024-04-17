namespace ClubService.Application.Dto;

public class ClubCreateUpdateDto(string name, bool isLocked, string subscriptionTier)
{
    public string Name { get; } = name;
    public bool IsLocked { get; } = isLocked;
    public string SubscriptionTier { get; } = subscriptionTier;
}