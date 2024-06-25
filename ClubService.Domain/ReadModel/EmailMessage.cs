namespace ClubService.Domain.ReadModel;

public class EmailMessage(Guid id, string recipientEMailAddress, string subject, string body, DateTime timestamp)
{
    public Guid Id { get; } = id;
    public string RecipientEMailAddress { get; } = recipientEMailAddress;
    public string Subject { get; } = subject;
    public string Body { get; } = body;
    public DateTime Timestamp { get; } = timestamp;
}