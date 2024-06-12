using System.Net;
using System.Text;
using ClubService.Application.Commands;
using ClubService.Domain.Event;
using ClubService.Domain.Event.Admin;
using ClubService.Domain.Model.Enum;
using ClubService.Domain.Model.ValueObject;
using Moq;
using Newtonsoft.Json;

namespace ClubService.IntegrationTests.EndToEnd;

[TestFixture]
public class AdminTests : TestBase
{
    private const string BaseUrl = "/api/v1.0/admins";

    [Test]
    public async Task
        GivenRegisterAdminCommand_WhenRegisterAdmin_ThenAdminRegisteredEventExistsInRepository()
    {
        // Given
        var numberOfEventsExpected = 1;
        var eventTypeExpected = EventType.ADMIN_REGISTERED;
        var entityTypeExpected = EntityType.ADMIN;
        var eventDataTypeExpected = typeof(AdminRegisteredEvent);
        var usernameExpected = "john_doe";
        var passwordExpected = "hunter2";
        var nameExpected = new FullName("John", "Doe");
        var statusExpected = AdminStatus.ACTIVE;
        var tennisClubIdExpected = new TennisClubId(new Guid("1fc64a89-9e63-4e9f-96f7-e2120f0ca6c3"));
        var registerAdminCommand = new AdminRegisterCommand(usernameExpected, passwordExpected, nameExpected.FirstName,
            nameExpected.LastName, tennisClubIdExpected.Id);
        var httpContent = new StringContent(JsonConvert.SerializeObject(registerAdminCommand), Encoding.UTF8,
            "application/json");

        MockAdminReadModelRepository
            .Setup(repo => repo.GetAdminsByTennisClubId(It.IsAny<Guid>()))
            .ReturnsAsync([]);

        // When
        var response = await HttpClient.PostAsync(BaseUrl, httpContent);

        // Then
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);
        var id = JsonConvert.DeserializeObject<Guid>(responseContent);

        var storedEvents = await EventRepository.GetEventsForEntity<IAdminDomainEvent>(id);
        Assert.That(storedEvents, Has.Count.EqualTo(numberOfEventsExpected));

        var storedEvent = storedEvents[numberOfEventsExpected - 1];
        Assert.Multiple(() =>
        {
            Assert.That(storedEvent.EventType, Is.EqualTo(eventTypeExpected));
            Assert.That(storedEvent.EntityType, Is.EqualTo(entityTypeExpected));
            Assert.That(storedEvent.EventData.GetType(), Is.EqualTo(eventDataTypeExpected));
        });

        var adminRegisteredEvent = (AdminRegisteredEvent)storedEvent.EventData;
        Assert.Multiple(() =>
        {
            Assert.That(adminRegisteredEvent.Name, Is.EqualTo(nameExpected));
            Assert.That(adminRegisteredEvent.Username, Is.EqualTo(usernameExpected));
            Assert.That(adminRegisteredEvent.Status, Is.EqualTo(statusExpected));
            Assert.That(adminRegisteredEvent.TennisClubId, Is.EqualTo(tennisClubIdExpected));
        });
    }

    [Test]
    public async Task GivenAdminId_WhenDeleteAdmin_ThenAdminDeletedEventExistsInRepositoryAndIdIsReturned()
    {
        // Given
        var numberOfEventsExpected = 2;
        var adminIdExpected = new Guid("1dd88382-f781-4bf8-94e3-05e99d1434fe");
        var eventTypeExpected = EventType.ADMIN_DELETED;
        var entityTypeExpected = EntityType.ADMIN;
        var eventDataTypeExpected = typeof(AdminDeletedEvent);

        // When
        var response = await HttpClient.DeleteAsync($"{BaseUrl}/{adminIdExpected}");

        // Then
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);
        var actualId = JsonConvert.DeserializeObject<Guid>(responseContent);
        Assert.That(actualId, Is.EqualTo(adminIdExpected));

        var storedEvents = await EventRepository.GetEventsForEntity<IAdminDomainEvent>(adminIdExpected);
        Assert.That(storedEvents, Has.Count.EqualTo(numberOfEventsExpected));

        var storedEvent = storedEvents[numberOfEventsExpected - 1];
        Assert.Multiple(() =>
        {
            Assert.That(storedEvent.EventType, Is.EqualTo(eventTypeExpected));
            Assert.That(storedEvent.EntityType, Is.EqualTo(entityTypeExpected));
            Assert.That(storedEvent.EntityId, Is.EqualTo(adminIdExpected));
            Assert.That(storedEvent.EventData.GetType(), Is.EqualTo(eventDataTypeExpected));
        });
    }

    [Test]
    public async Task GivenDeletedAdminId_WhenDeleteAdminAgain_ThenErrorResponseIsReturned()
    {
        // Given
        var deletedAdminId = new Guid("5d2f1aec-1cc6-440a-b04f-ba8b3085a35a");

        // When
        var response = await HttpClient.DeleteAsync($"{BaseUrl}/{deletedAdminId}");

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);
        Assert.That(responseContent, Does.Contain("Admin is already deleted!"));
    }

    [Test]
    public async Task GivenUpdateAdminCommand_WhenAdminFullNameChanged_ThenAdminFullNameChangedEventExistsInRepository()
    {
        // Given
        var numberOfEventsExpected = 2;
        var adminIdExpected = new Guid("1dd88382-f781-4bf8-94e3-05e99d1434fe");
        var eventTypeExpected = EventType.ADMIN_FULL_NAME_CHANGED;
        var entityTypeExpected = EntityType.ADMIN;
        var eventDataTypeExpected = typeof(AdminFullNameChangedEvent);
        var nameExpected = new FullName("Jane", "Doe");
        var updateAdminCommand = new AdminUpdateCommand(nameExpected.FirstName,
            nameExpected.LastName);
        var httpContent = new StringContent(JsonConvert.SerializeObject(updateAdminCommand), Encoding.UTF8,
            "application/json");

        // When
        var response = await HttpClient.PatchAsync($"{BaseUrl}/{adminIdExpected}", httpContent);

        // Then
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);
        var actualId = JsonConvert.DeserializeObject<Guid>(responseContent);
        Assert.That(actualId, Is.EqualTo(adminIdExpected));

        var storedEvents = await EventRepository.GetEventsForEntity<IAdminDomainEvent>(adminIdExpected);
        Assert.That(storedEvents, Has.Count.EqualTo(numberOfEventsExpected));

        var storedEvent = storedEvents[numberOfEventsExpected - 1];
        Assert.Multiple(() =>
        {
            Assert.That(storedEvent.EventType, Is.EqualTo(eventTypeExpected));
            Assert.That(storedEvent.EntityType, Is.EqualTo(entityTypeExpected));
            Assert.That(storedEvent.EntityId, Is.EqualTo(adminIdExpected));
            Assert.That(storedEvent.EventData.GetType(), Is.EqualTo(eventDataTypeExpected));
        });
    }
}