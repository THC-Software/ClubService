using ClubService.Domain.Model.ValueObject;

namespace ClubService.Domain.ReadModel;

public class SubscriptionTierReadModel(SubscriptionTierId id, string name, int maxMemberCount)
{
    public SubscriptionTierId Id { get; } = id;
    public string Name { get; } = name;
    public int MaxMemberCount { get; } = maxMemberCount;
}