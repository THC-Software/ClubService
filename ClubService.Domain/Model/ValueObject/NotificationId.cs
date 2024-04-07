namespace ClubService.Domain.Model.ValueObject;

public class NotificationId
{
    public string Id { get; private set; }

    private NotificationId() { }
    
    public NotificationId(string id)
    {
        Id = id;
    }

    protected bool Equals(NotificationId other)
    {
        return Id == other.Id;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((NotificationId)obj);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}