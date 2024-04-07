namespace ClubService.Domain.Model;

public class TennisClub
{
    public TennisClubId Id { get; private set; }
    public string Name { get; private set; }
    public bool IsLocked { get; private set; }
    public SubscriptionTierId SubscriptionTierId { get; private set; }
    public List<MemberId> Members { get; private set; }
    
    private TennisClub() { }

    private TennisClub(TennisClubId id, string name, bool isLocked, SubscriptionTierId subscriptionTierId)
    {
        Id = id;
        Name = name;
        IsLocked = isLocked;
        SubscriptionTierId = subscriptionTierId;
        Members = new List<MemberId>();
    }

    public static TennisClub Create(TennisClubId id, string name, bool isLocked, SubscriptionTierId subscriptionTierId)
    {
        return new TennisClub(id, name, isLocked, subscriptionTierId);
    }

    protected bool Equals(TennisClub other)
    {
        return Id.Equals(other.Id);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((TennisClub)obj);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}