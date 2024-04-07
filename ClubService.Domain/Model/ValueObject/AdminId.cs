namespace ClubService.Domain.Model.ValueObject;

public class AdminId
{
    public string Id { get; private set; }

    public AdminId(string id)
    {
        Id = id;
    }

    protected bool Equals(AdminId other)
    {
        return Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((AdminId)obj);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}