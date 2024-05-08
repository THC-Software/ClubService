using System.Text;
using ClubService.Application.Commands;
using ClubService.Domain.Event;
using ClubService.Domain.Event.TennisClub;
using Newtonsoft.Json;

namespace ClubService.IntegrationTests;

[TestFixture]
public class TennisClubTests : TestBase
{
    [Test]
    public async Task
        GivenRegisterTennisClubCommand_WhenRegisterTennisClub_ThenRegisterTennisClubEventExistsInRepository()
    {
        // Given
        var eventTypeExpected = EventType.TENNIS_CLUB_REGISTERED;
        var entityTypeExpected = EntityType.TENNIS_CLUB;
        var eventDataTypeExpected = typeof(TennisClubRegisteredEvent);
        var endpointUrl = "/api/v1.0/tennisClubs/";
        var registerTennisClubCommand = new TennisClubRegisterCommand("Test", Guid.NewGuid().ToString());
        var httpContent = new StringContent(JsonConvert.SerializeObject(registerTennisClubCommand), Encoding.UTF8,
            "application/json");
        
        // When
        var response = await HttpClient.PostAsync(endpointUrl, httpContent);
        
        // Then
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var storedEvents =
            PostgresEventRepository
                .GetEventsForEntity<
                    ITennisClubDomainEvent>(
                    Guid.Parse(responseContent));
        Assert.That(storedEvents, Has.Count.EqualTo(1));
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
}