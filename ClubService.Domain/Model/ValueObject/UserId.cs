namespace ClubService.Domain.Model.ValueObject;

public class UserId(Guid id)
{
    public Guid Id { get; } = id;

    protected bool Equals(UserId other)
    {
        return Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((UserId)obj);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}