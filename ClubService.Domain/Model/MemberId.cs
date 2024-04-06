namespace ClubService.Domain.Model;

public class MemberId
{
    public string Id { get; private set; }

    public MemberId(string id)
    {
        Id = id;
    }

    protected bool Equals(MemberId other)
    {
        return Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((MemberId)obj);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}