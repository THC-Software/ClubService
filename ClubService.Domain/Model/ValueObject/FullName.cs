namespace ClubService.Domain.Model.ValueObject;

public class FullName(string firstName, string lastName)
{
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;

    protected bool Equals(FullName other)
    {
        return FirstName == other.FirstName && LastName == other.LastName;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((FullName)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(FirstName, LastName);
    }
}