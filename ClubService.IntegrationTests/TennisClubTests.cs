using System.Text;
using ClubService.Application.Commands;
using ClubService.Domain.Event;
using ClubService.Domain.Event.TennisClub;
using Newtonsoft.Json;

namespace ClubService.IntegrationTests;

[TestFixture]
public class TennisClubTests : TestBase
{
    private const string BaseUrl = "/api/v1.0/tennisClubs";
    
    [Test]
    public async Task
        GivenRegisterTennisClubCommand_WhenRegisterTennisClub_ThenRegisterTennisClubEventExistsInRepository()
    {
        // Given
        var numberOfEventsExpected = 1;
        var eventTypeExpected = EventType.TENNIS_CLUB_REGISTERED;
        var entityTypeExpected = EntityType.TENNIS_CLUB;
        var eventDataTypeExpected = typeof(TennisClubRegisteredEvent);
        var registerTennisClubCommand = new TennisClubRegisterCommand("Test", Guid.NewGuid().ToString());
        var httpContent = new StringContent(JsonConvert.SerializeObject(registerTennisClubCommand), Encoding.UTF8,
            "application/json");
        
        // When
        var response = await HttpClient.PostAsync(BaseUrl, httpContent);
        
        // Then
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);
        
        var storedEvents =
            PostgresEventRepository
                .GetEventsForEntity<
                    ITennisClubDomainEvent>(
                    Guid.Parse(responseContent));
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
            Assert.That(tennisClubRegisteredEventActual.IsLocked, Is.False);
            Assert.That(tennisClubRegisteredEventActual.SubscriptionTierId.Id.ToString(),
                Is.EqualTo(registerTennisClubCommand.SubscriptionTierId));
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
        
        var storedEvents = PostgresEventRepository.GetEventsForEntity<ITennisClubDomainEvent>(clubIdExpected);
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
        
        var storedEvents = PostgresEventRepository.GetEventsForEntity<ITennisClubDomainEvent>(clubIdExpected);
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
}