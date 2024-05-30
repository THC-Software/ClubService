using System.Text;
using ClubService.Application.Commands;
using ClubService.Domain.Event;
using ClubService.Domain.Event.TennisClub;
using ClubService.Domain.Model.Enum;
using ClubService.IntegrationTests.TestSetup;
using Newtonsoft.Json;

namespace ClubService.IntegrationTests.EndToEnd;

[TestFixture]
public class TennisClubTests : TestBase
{
    private const string BaseUrl = "/api/v1.0/tennisClubs";
    
    [Test]
    public async Task
        GivenRegisterTennisClubCommand_WhenRegisterTennisClub_ThenTennisClubRegisteredEventExistsInRepository()
    {
        // Given
        var numberOfEventsExpected = 1;
        var eventTypeExpected = EventType.TENNIS_CLUB_REGISTERED;
        var entityTypeExpected = EntityType.TENNIS_CLUB;
        var eventDataTypeExpected = typeof(TennisClubRegisteredEvent);
        var subscriptionTierIdExpected = new Guid("38888969-d579-46ec-9cd6-0208569a077e");
        var registerTennisClubCommand = new TennisClubRegisterCommand("Test", subscriptionTierIdExpected.ToString());
        var httpContent = new StringContent(JsonConvert.SerializeObject(registerTennisClubCommand), Encoding.UTF8,
            "application/json");
        
        // When
        var response = await HttpClient.PostAsync(BaseUrl, httpContent);
        
        // Then
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);
        
        var storedEvents =
            await EventRepository.GetEventsForEntity<ITennisClubDomainEvent>(Guid.Parse(responseContent));
        Assert.That(storedEvents, Has.Count.EqualTo(numberOfEventsExpected));
        var storedEvent = storedEvents[0];
        Assert.Multiple(() =>
        {
            Assert.That(storedEvent.EventType, Is.EqualTo(eventTypeExpected));
            Assert.That(storedEvent.EntityType, Is.EqualTo(entityTypeExpected));
            Assert.That(storedEvent.EventData.GetType(), Is.EqualTo(eventDataTypeExpected));
        });
        var tennisClubRegisteredEventActual = (TennisClubRegisteredEvent)storedEvent.EventData;
        Assert.Multiple(() =>
        {
            Assert.That(tennisClubRegisteredEventActual.Name, Is.EqualTo(registerTennisClubCommand.Name));
            Assert.That(tennisClubRegisteredEventActual.Status, Is.EqualTo(TennisClubStatus.ACTIVE));
            Assert.That(tennisClubRegisteredEventActual.SubscriptionTierId.Id,
                Is.EqualTo(subscriptionTierIdExpected));
        });
    }
    
    [Test]
    public async Task GivenTennisClubId_WhenLockTennisClub_ThenTennisClubLockedEventExistsInRepositoryAndIdIsReturned()
    {
        // Given
        var numberOfEventsExpected = 2;
        var clubIdExpected = new Guid("1fc64a89-9e63-4e9f-96f7-e2120f0ca6c3");
        var eventTypeExpected = EventType.TENNIS_CLUB_LOCKED;
        var entityTypeExpected = EntityType.TENNIS_CLUB;
        var eventDataTypeExpected = typeof(TennisClubLockedEvent);
        
        // When
        var response = await HttpClient.PostAsync($"{BaseUrl}/{clubIdExpected.ToString()}/lock", null);
        
        // Then
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);
        Assert.That(responseContent, Is.EqualTo(clubIdExpected.ToString()));
        
        var storedEvents = await EventRepository.GetEventsForEntity<ITennisClubDomainEvent>(clubIdExpected);
        Assert.That(storedEvents, Has.Count.EqualTo(numberOfEventsExpected));
        
        var storedEvent = storedEvents[1];
        Assert.Multiple(() =>
        {
            Assert.That(storedEvent.EventType, Is.EqualTo(eventTypeExpected));
            Assert.That(storedEvent.EntityType, Is.EqualTo(entityTypeExpected));
            Assert.That(storedEvent.EntityId, Is.EqualTo(clubIdExpected));
            Assert.That(storedEvent.EventData.GetType(), Is.EqualTo(eventDataTypeExpected));
        });
    }
    
    [Test]
    public async Task
        GivenTennisClubId_WhenUnlockTennisClub_ThenTennisClubUnlockedEventExistsInRepositoryAndIdIsReturned()
    {
        // Given
        var numberOfEventsExpected = 3;
        var clubIdExpected = new Guid("6a463e1a-6b0f-4825-83c3-911f12f80076");
        var eventTypeExpected = EventType.TENNIS_CLUB_UNLOCKED;
        var entityTypeExpected = EntityType.TENNIS_CLUB;
        var eventDataTypeExpected = typeof(TennisClubUnlockedEvent);
        
        // When
        var response = await HttpClient.DeleteAsync($"{BaseUrl}/{clubIdExpected.ToString()}/lock");
        
        // Then
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);
        Assert.That(responseContent, Is.EqualTo(clubIdExpected.ToString()));
        
        var storedEvents = await EventRepository.GetEventsForEntity<ITennisClubDomainEvent>(clubIdExpected);
        Assert.That(storedEvents, Has.Count.EqualTo(numberOfEventsExpected));
        
        var storedEvent = storedEvents[2];
        Assert.Multiple(() =>
        {
            Assert.That(storedEvent.EventType, Is.EqualTo(eventTypeExpected));
            Assert.That(storedEvent.EntityType, Is.EqualTo(entityTypeExpected));
            Assert.That(storedEvent.EntityId, Is.EqualTo(clubIdExpected));
            Assert.That(storedEvent.EventData.GetType(), Is.EqualTo(eventDataTypeExpected));
        });
    }
    
    [Test]
    public async Task
        GivenUpdateTennisClubCommand_WhenUpdateTennisClub_ThenTennisClubSubscriptionTierChangedEventExistsInRepositoryAndIdIsReturned()
    {
        // Given
        var numberOfEventsExpected = 2;
        var clubIdExpected = new Guid("1fc64a89-9e63-4e9f-96f7-e2120f0ca6c3");
        var eventTypeExpected = EventType.TENNIS_CLUB_SUBSCRIPTION_TIER_CHANGED;
        var entityTypeExpected = EntityType.TENNIS_CLUB;
        var eventDataTypeExpected = typeof(TennisClubSubscriptionTierChangedEvent);
        var tennisClubUpdateCommand = new TennisClubUpdateCommand(null, Guid.NewGuid().ToString());
        var jsonContent = JsonConvert.SerializeObject(tennisClubUpdateCommand);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        
        // When
        var response = await HttpClient.PatchAsync($"{BaseUrl}/{clubIdExpected.ToString()}", content);
        
        // Then
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);
        Assert.That(responseContent, Is.EqualTo(clubIdExpected.ToString()));
        
        var storedEvents = await EventRepository.GetEventsForEntity<ITennisClubDomainEvent>(clubIdExpected);
        Assert.That(storedEvents, Has.Count.EqualTo(numberOfEventsExpected));
        
        var storedEvent = storedEvents[1];
        Assert.Multiple(() =>
        {
            Assert.That(storedEvent.EventType, Is.EqualTo(eventTypeExpected));
            Assert.That(storedEvent.EntityType, Is.EqualTo(entityTypeExpected));
            Assert.That(storedEvent.EntityId, Is.EqualTo(clubIdExpected));
            Assert.That(storedEvent.EventData.GetType(), Is.EqualTo(eventDataTypeExpected));
        });
        
        var tennisClubSubscriptionTierChangedEventActual =
            (TennisClubSubscriptionTierChangedEvent)storedEvent.EventData;
        Assert.That(tennisClubSubscriptionTierChangedEventActual.SubscriptionTierId.Id.ToString(),
            Is.EqualTo(tennisClubUpdateCommand.SubscriptionTierId));
    }
    
    [Test]
    public async Task
        GivenUpdateTennisClubCommand_WhenUpdateTennisClub_ThenTennisClubNameChangedEventExistsInRepositoryAndIdIsReturned()
    {
        // Given
        var numberOfEventsExpected = 2;
        var clubIdExpected = new Guid("1fc64a89-9e63-4e9f-96f7-e2120f0ca6c3");
        var eventTypeExpected = EventType.TENNIS_CLUB_NAME_CHANGED;
        var entityTypeExpected = EntityType.TENNIS_CLUB;
        var eventDataTypeExpected = typeof(TennisClubNameChangedEvent);
        var tennisClubUpdateCommand = new TennisClubUpdateCommand("Some new name", null);
        var jsonContent = JsonConvert.SerializeObject(tennisClubUpdateCommand);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        
        // When
        var response = await HttpClient.PatchAsync($"{BaseUrl}/{clubIdExpected.ToString()}", content);
        
        // Then
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);
        Assert.That(responseContent, Is.EqualTo(clubIdExpected.ToString()));
        
        var storedEvents = await EventRepository.GetEventsForEntity<ITennisClubDomainEvent>(clubIdExpected);
        Assert.That(storedEvents, Has.Count.EqualTo(numberOfEventsExpected));
        
        var storedEvent = storedEvents[1];
        Assert.Multiple(() =>
        {
            Assert.That(storedEvent.EventType, Is.EqualTo(eventTypeExpected));
            Assert.That(storedEvent.EntityType, Is.EqualTo(entityTypeExpected));
            Assert.That(storedEvent.EntityId, Is.EqualTo(clubIdExpected));
            Assert.That(storedEvent.EventData.GetType(), Is.EqualTo(eventDataTypeExpected));
        });
        
        var tennisClubNameChangedEvent =
            (TennisClubNameChangedEvent)storedEvent.EventData;
        Assert.That(tennisClubNameChangedEvent.Name,
            Is.EqualTo(tennisClubUpdateCommand.Name));
    }
    
    [Test]
    public async Task
        GivenTennisClubId_WhenDeleteTennisClub_ThenTennisClubDeletedEventExistsInRepositoryAndIdIsReturned()
    {
        // Given
        var numberOfEventsExpected = 2;
        var clubIdExpected = new Guid("1fc64a89-9e63-4e9f-96f7-e2120f0ca6c3");
        var eventTypeExpected = EventType.TENNIS_CLUB_DELETED;
        var entityTypeExpected = EntityType.TENNIS_CLUB;
        var eventDataTypeExpected = typeof(TennisClubDeletedEvent);
        
        // When
        var response = await HttpClient.DeleteAsync($"{BaseUrl}/{clubIdExpected.ToString()}");
        
        // Then
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);
        Assert.That(responseContent, Is.EqualTo(clubIdExpected.ToString()));
        
        var storedEvents = await EventRepository.GetEventsForEntity<ITennisClubDomainEvent>(clubIdExpected);
        Assert.That(storedEvents, Has.Count.EqualTo(numberOfEventsExpected));
        
        var storedEvent = storedEvents[1];
        Assert.Multiple(() =>
        {
            Assert.That(storedEvent.EventType, Is.EqualTo(eventTypeExpected));
            Assert.That(storedEvent.EntityType, Is.EqualTo(entityTypeExpected));
            Assert.That(storedEvent.EntityId, Is.EqualTo(clubIdExpected));
            Assert.That(storedEvent.EventData.GetType(), Is.EqualTo(eventDataTypeExpected));
        });
    }
}