namespace ClubService.Domain.ReadModel;

public class ProcessedEvent(Guid eventId)
{
    public Guid EventId { get; } = eventId;
}