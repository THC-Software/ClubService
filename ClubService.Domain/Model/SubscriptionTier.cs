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

    protected bool Equals(SubscriptionTier other)
    {
        return Id.Equals(other.Id);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((SubscriptionTier)obj);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}