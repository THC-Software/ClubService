using System.Net.Http.Headers;
using System.Text;
using ClubService.Application.Commands;
using ClubService.Domain.Event;
using ClubService.Domain.Event.TennisClub;
using ClubService.Domain.Model.Entity;
using ClubService.Domain.Model.Enum;
using ClubService.Domain.Model.ValueObject;
using Moq;
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
        const string adminUsername = "testuser";
        const string adminPassword = "test";
        const string adminFirstName = "John";
        const string adminLastName = "Doe";
        var subscriptionTierIdExpected = new Guid("38888969-d579-46ec-9cd6-0208569a077e");
        var registerTennisClubCommand = new TennisClubRegisterCommand("Test", subscriptionTierIdExpected,
            adminUsername, adminPassword, adminFirstName, adminLastName);
        var httpContent = new StringContent(JsonConvert.SerializeObject(registerTennisClubCommand), Encoding.UTF8,
            "application/json");
        
        MockLoginRepository
            .Setup(repo => repo.Add(It.IsAny<UserPassword>()))
            .Returns(Task.CompletedTask);

        // When
        var response = await HttpClient.PostAsync(BaseUrl, httpContent);

        // Then
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);
        var clubId = JsonConvert.DeserializeObject<Guid>(responseContent);

        var storedEvents =
            await EventRepository.GetEventsForEntity<ITennisClubDomainEvent>(clubId, EntityType.TENNIS_CLUB);
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
        var jwtToken = JwtTokenHelper.GenerateJwtToken("", "", ["SYSTEM_OPERATOR"]);

        // When
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        var response = await HttpClient.PostAsync($"{BaseUrl}/{clubIdExpected}/lock", null);

        // Then
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);
        var actualId = JsonConvert.DeserializeObject<Guid>(responseContent);
        Assert.That(actualId, Is.EqualTo(clubIdExpected));

        var storedEvents =
            await EventRepository.GetEventsForEntity<ITennisClubDomainEvent>(clubIdExpected, EntityType.TENNIS_CLUB);
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
        var jwtToken = JwtTokenHelper.GenerateJwtToken("", "", ["SYSTEM_OPERATOR"]);

        // When
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        var response = await HttpClient.DeleteAsync($"{BaseUrl}/{clubIdExpected}/lock");

        // Then
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);
        var actualId = JsonConvert.DeserializeObject<Guid>(responseContent);
        Assert.That(actualId, Is.EqualTo(clubIdExpected));

        var storedEvents =
            await EventRepository.GetEventsForEntity<ITennisClubDomainEvent>(clubIdExpected, EntityType.TENNIS_CLUB);
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
        var subscriptionTierId = new SubscriptionTierId(new Guid("38888969-d579-46ec-9cd6-0208569a077e"));
        var tennisClubUpdateCommand = new TennisClubUpdateCommand(null, subscriptionTierId.Id);
        var jsonContent = JsonConvert.SerializeObject(tennisClubUpdateCommand);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var adminId = new Guid("5d2f1aec-1cc6-440a-b04f-ba8b3085a35a");
        var jwtToken =
            JwtTokenHelper.GenerateJwtToken(adminId.ToString(), clubIdExpected.ToString(), ["ADMIN"]);

        // When
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        var response = await HttpClient.PatchAsync($"{BaseUrl}/{clubIdExpected}", content);

        // Then
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);
        var actualId = JsonConvert.DeserializeObject<Guid>(responseContent);
        Assert.That(actualId, Is.EqualTo(clubIdExpected));

        var storedEvents =
            await EventRepository.GetEventsForEntity<ITennisClubDomainEvent>(clubIdExpected, EntityType.TENNIS_CLUB);
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
        Assert.That(tennisClubSubscriptionTierChangedEventActual.SubscriptionTierId.Id,
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

        var adminId = new Guid("5d2f1aec-1cc6-440a-b04f-ba8b3085a35a");
        var jwtToken =
            JwtTokenHelper.GenerateJwtToken(adminId.ToString(), clubIdExpected.ToString(), ["ADMIN"]);

        // When
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        var response = await HttpClient.PatchAsync($"{BaseUrl}/{clubIdExpected}", content);

        // Then
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);
        var actualId = JsonConvert.DeserializeObject<Guid>(responseContent);
        Assert.That(actualId, Is.EqualTo(clubIdExpected));

        var storedEvents =
            await EventRepository.GetEventsForEntity<ITennisClubDomainEvent>(clubIdExpected, EntityType.TENNIS_CLUB);
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

        var adminId = new Guid("5d2f1aec-1cc6-440a-b04f-ba8b3085a35a");
        var jwtToken =
            JwtTokenHelper.GenerateJwtToken(adminId.ToString(), clubIdExpected.ToString(), ["ADMIN"]);

        // When
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        var response = await HttpClient.DeleteAsync($"{BaseUrl}/{clubIdExpected}");

        // Then
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);
        var actualId = JsonConvert.DeserializeObject<Guid>(responseContent);
        Assert.That(actualId, Is.EqualTo(clubIdExpected));

        var storedEvents =
            await EventRepository.GetEventsForEntity<ITennisClubDomainEvent>(clubIdExpected, EntityType.TENNIS_CLUB);
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