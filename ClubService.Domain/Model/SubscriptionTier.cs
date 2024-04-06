namespace ClubService.Domain.Model;

public class SubscriptionTier
{
    public SubscriptionTierId Id { get; private set; }
    public string Name { get; private set; }
    public int MaxMemberCount { get; private set; }

    private SubscriptionTier() { }

    private SubscriptionTier(SubscriptionTierId id, string name, int maxMemberCount)
    {
        Id = id;
        Name = name;
        MaxMemberCount = maxMemberCount;
    }

    public static SubscriptionTier Create(SubscriptionTierId id, string name, int maxMemberCount)
    {
        return new SubscriptionTier(id, name, maxMemberCount);
    }
}