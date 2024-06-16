using System.Text.Json;
using ClubService.Domain.Event;

namespace ClubService.Infrastructure.EventHandling;

public static class EventParser
{
    public static DomainEnvelope<IDomainEvent> ParseEvent(JsonElement jsonEvent)
    {
        var eventIdStr = jsonEvent.GetProperty("eventId").GetString();
        var entityIdStr = jsonEvent.GetProperty("entityId").GetString();
        var eventTypeStr = jsonEvent.GetProperty("eventType").GetString();
        var entityTypeStr = jsonEvent.GetProperty("entityType").GetString();
        var timestampStr = jsonEvent.GetProperty("timestamp").GetString();
        var eventData = jsonEvent.GetProperty("eventData").GetString();

        if (eventIdStr == null ||
            entityIdStr == null ||
            eventTypeStr == null ||
            entityTypeStr == null ||
            timestampStr == null ||
            eventData == null)
        {
            throw new InvalidOperationException("event has missing properties");
        }

        var eventId = Guid.Parse(eventIdStr);
        var entityId = Guid.Parse(entityIdStr);
        var eventType = (EventType)Enum.Parse(typeof(EventType), eventTypeStr);
        var entityType = (EntityType)Enum.Parse(typeof(EntityType), entityTypeStr);
        var timestamp = DateTime.Parse(timestampStr).ToUniversalTime();
        var deserializedEventData = EventDeserializer.DeserializeEventData<IDomainEvent>(eventType, eventData);

        return new DomainEnvelope<IDomainEvent>(
            eventId,
            entityId,
            eventType,
            entityType,
            timestamp,
            deserializedEventData);
    }
}