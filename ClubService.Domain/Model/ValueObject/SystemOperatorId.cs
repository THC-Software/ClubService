namespace ClubService.Domain.Model.ValueObject;

public class SystemOperatorId(Guid id)
{
    public Guid Id { get; } = id;
    
    protected bool Equals(SystemOperatorId other)
    {
        return Id == other.Id;
    }
    
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
    
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((SystemOperatorId)obj);
    }
}