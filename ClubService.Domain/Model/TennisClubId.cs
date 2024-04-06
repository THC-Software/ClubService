namespace ClubService.Domain.Model;

public class TennisClubId
{
    public string Id { get; }

    public TennisClubId(string id)
    {
        Id = id;
    }

    protected bool Equals(TennisClubId other)
    {
        return Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((TennisClubId)obj);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}