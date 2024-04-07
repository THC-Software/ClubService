namespace ClubService.Domain.Model.ValueObject;

public class FullName
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }

    public FullName(string firstName, string lastName)
    {
        LastName = lastName;
        FirstName = firstName;
    }

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