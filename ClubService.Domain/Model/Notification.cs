namespace ClubService.Domain.Model;

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
}