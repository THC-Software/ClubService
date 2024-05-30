using System.Text.Json.Nodes;
using ClubService.Domain.Event;

namespace ClubService.Infrastructure.EventHandling;

public class EventParser
{
    public static DomainEnvelope<IDomainEvent> ParseEvent(JsonNode jsonEvent)
    {
        //TODO: what if some parameters are null?
        var eventId = Guid.Parse(jsonEvent["eventId"]?.GetValue<string>());
        var entityId = Guid.Parse(jsonEvent["entityId"].GetValue<string>());
        var eventType = (EventType)Enum.Parse(typeof(EventType), jsonEvent["eventType"].GetValue<string>());
        var entityType = (EntityType)Enum.Parse(typeof(EntityType), jsonEvent["entityType"].GetValue<string>());
        var timestamp = DateTime.Parse(jsonEvent["timestamp"].GetValue<string>()).ToUniversalTime();
        var eventData = jsonEvent["eventData"].GetValue<string>();
        var deserializedEventData = EventDeserializer.DeserializeEventData<IDomainEvent>(eventType, eventData);
        
        return new DomainEnvelope<IDomainEvent>(
            eventId: eventId,
            entityId: entityId,
            eventType: eventType,
            entityType: entityType,
            timestamp: timestamp,
            eventData: deserializedEventData);
    }
}