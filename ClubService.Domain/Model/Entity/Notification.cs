using ClubService.Domain.Model.ValueObject;

namespace ClubService.Domain.Model.Entity;

public class Notification
{
    public NotificationId Id { get; private set; }
    public string Title { get; private set; }
    public string Text { get; private set; }
    
    private Notification() { }

    private Notification(NotificationId id, string title, string text)
    {
        Id = id;
        Title = title;
        Text = text;
    }

    public static Notification Create(NotificationId id, string title, string text)
    {
        return new Notification(id, title, text);
    }

    protected bool Equals(Notification other)
    {
        return Id.Equals(other.Id);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Notification)obj);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}