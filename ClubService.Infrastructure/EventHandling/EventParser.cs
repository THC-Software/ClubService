using System.Text.Json.Nodes;
using ClubService.Domain.Event;

namespace ClubService.Infrastructure.EventHandling;

public class EventParser
{
    public static DomainEnvelope<IDomainEvent> ParseEvent(JsonNode jsonEvent)
    {
        var jsonEventId = jsonEvent["eventId"];
        var jsonEntityId = jsonEvent["entityId"];
        var jsonEventType = jsonEvent["eventType"];
        var jsonEntityType = jsonEvent["entityType"];
        var jsonTimestamp = jsonEvent["timestamp"];
        var jsonEventData = jsonEvent["eventData"];
        
        if (jsonEventId == null ||
            jsonEntityId == null ||
            jsonEventType == null ||
            jsonEntityType == null ||
            jsonTimestamp == null ||
            jsonEventData == null)
        {
            throw new InvalidOperationException("event has missing properties");
        }
        
        var eventId = Guid.Parse(jsonEventId.GetValue<string>());
        var entityId = Guid.Parse(jsonEntityId.GetValue<string>());
        var eventType = (EventType)Enum.Parse(typeof(EventType), jsonEventType.GetValue<string>());
        var entityType = (EntityType)Enum.Parse(typeof(EntityType), jsonEntityType.GetValue<string>());
        var timestamp = DateTime.Parse(jsonTimestamp.GetValue<string>()).ToUniversalTime();
        var eventData = jsonEventData.GetValue<string>();
        
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