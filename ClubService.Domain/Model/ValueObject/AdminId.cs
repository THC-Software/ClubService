namespace ClubService.Domain.Model.ValueObject;

public class AdminId(Guid id)
{
    public Guid Id { get; } = id;

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