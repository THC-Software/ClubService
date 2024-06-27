using System.Net;
using System.Net.Http.Headers;
using System.Text;
using ClubService.Application.Commands;
using ClubService.Domain.Event;
using ClubService.Domain.Event.Member;
using ClubService.Domain.Event.SubscriptionTier;
using ClubService.Domain.Event.TennisClub;
using ClubService.Domain.Model.Enum;
using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.ReadModel;
using Moq;
using Newtonsoft.Json;

namespace ClubService.IntegrationTests.EndToEnd;

[TestFixture]
public class MemberTests : TestBase
{
    private const string BaseUrl = "/api/v1.0/members";

    [Test]
    public async Task GivenRegisterMemberCommand_WhenRegisterMember_ThenMemberRegisteredEventExistsInRepository()
    {
        // Given
        var tennisClubRegisteredEvent = new TennisClubRegisteredEvent(
            new TennisClubId(Guid.NewGuid()),
            "Sample Tennis Club",
            new SubscriptionTierId(Guid.NewGuid()),
            TennisClubStatus.ACTIVE
        );
        var tennisClubReadModel = TennisClubReadModel.FromDomainEvent(tennisClubRegisteredEvent);
        MockTennisClubReadModelRepository
            .Setup(repo => repo.GetTennisClubById(It.IsAny<Guid>()))
            .ReturnsAsync(tennisClubReadModel);

        var subscriptionTierCreatedEvent = new SubscriptionTierCreatedEvent(
            new SubscriptionTierId(Guid.NewGuid()),
            "Standard",
            10
        );
        var subscriptionTierReadModel = SubscriptionTierReadModel.FromDomainEvent(subscriptionTierCreatedEvent);
        MockSubscriptionTierReadModelRepository
            .Setup(repo => repo.GetSubscriptionTierById(It.IsAny<Guid>()))
            .ReturnsAsync(subscriptionTierReadModel);

        MockMemberReadModelRepository
            .Setup(repo => repo.GetMembersByTennisClubId(It.IsAny<Guid>()))
            .ReturnsAsync([]);

        var numberOfEventsExpected = 1;
        var eventTypeExpected = EventType.MEMBER_REGISTERED;
        var entityTypeExpected = EntityType.MEMBER;
        var eventDataTypeExpected = typeof(MemberRegisteredEvent);
        var nameExpected = new FullName("John", "Doe");
        var passwordExpected = "CorrectBatteryHorseStaple";
        var statusExpected = MemberStatus.ACTIVE;
        var tennisClubIdExpected = new TennisClubId(new Guid("1fc64a89-9e63-4e9f-96f7-e2120f0ca6c3"));
        var registerMemberCommand = new MemberRegisterCommand(nameExpected.FirstName, nameExpected.LastName,
            "john.doe@dev.com", passwordExpected, tennisClubIdExpected.Id);

        var jwtToken = JwtTokenHelper.GenerateJwtToken("", tennisClubIdExpected.Id.ToString(), ["ADMIN"]);
        var httpContent = new StringContent(JsonConvert.SerializeObject(registerMemberCommand), Encoding.UTF8,
            "application/json");

        // When
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        var response = await HttpClient.PostAsync(BaseUrl, httpContent);

        // Then
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);
        var id = JsonConvert.DeserializeObject<Guid>(responseContent);

        var storedEvents = await EventRepository.GetEventsForEntity<IMemberDomainEvent>(id, EntityType.MEMBER);
        Assert.That(storedEvents, Has.Count.EqualTo(numberOfEventsExpected));

        var storedEvent = storedEvents[numberOfEventsExpected - 1];
        Assert.Multiple(() =>
        {
            Assert.That(storedEvent.EventType, Is.EqualTo(eventTypeExpected));
            Assert.That(storedEvent.EntityType, Is.EqualTo(entityTypeExpected));
            Assert.That(storedEvent.EventData.GetType(), Is.EqualTo(eventDataTypeExpected));
        });

        var memberRegisteredEvent = (MemberRegisteredEvent)storedEvent.EventData;
        Assert.Multiple(() =>
        {
            Assert.That(memberRegisteredEvent.Name, Is.EqualTo(nameExpected));
            Assert.That(memberRegisteredEvent.Email, Is.EqualTo(registerMemberCommand.Email));
            Assert.That(memberRegisteredEvent.Status, Is.EqualTo(statusExpected));
            Assert.That(memberRegisteredEvent.TennisClubId, Is.EqualTo(tennisClubIdExpected));
        });
    }

    [Test]
    public async Task GivenMemberId_WhenLockMember_ThenMemberLockedEventExistsInRepositoryAndIdIsReturned()
    {
        // Given
        var numberOfEventsExpected = 2;
        var memberIdExpected = new Guid("60831440-06d2-4017-9a7b-016e9cd0b2dc");
        var eventTypeExpected = EventType.MEMBER_LOCKED;
        var entityTypeExpected = EntityType.MEMBER;
        var eventDataTypeExpected = typeof(MemberLockedEvent);
        
        var tennisClubIdExpected = new TennisClubId(new Guid("1fc64a89-9e63-4e9f-96f7-e2120f0ca6c3"));
        var jwtToken = JwtTokenHelper.GenerateJwtToken("", tennisClubIdExpected.Id.ToString(), ["ADMIN"]);

        // When
        var response = await HttpClient.PostAsync($"{BaseUrl}/{memberIdExpected.ToString()}/lock", null);

        // Then
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);
        var actualId = JsonConvert.DeserializeObject<Guid>(responseContent);
        Assert.That(actualId, Is.EqualTo(memberIdExpected));

        var storedEvents =
            await EventRepository.GetEventsForEntity<IMemberDomainEvent>(memberIdExpected, EntityType.MEMBER);
        Assert.That(storedEvents, Has.Count.EqualTo(numberOfEventsExpected));

        var storedEvent = storedEvents[numberOfEventsExpected - 1];
        Assert.Multiple(() =>
        {
            Assert.That(storedEvent.EventType, Is.EqualTo(eventTypeExpected));
            Assert.That(storedEvent.EntityType, Is.EqualTo(entityTypeExpected));
            Assert.That(storedEvent.EntityId, Is.EqualTo(memberIdExpected));
            Assert.That(storedEvent.EventData.GetType(), Is.EqualTo(eventDataTypeExpected));
        });
    }

    [Test]
    public async Task GivenMemberId_WhenUnlockMember_ThenMemberUnlockedEventExistsInRepositoryAndIdIsReturned()
    {
        // Given
        var numberOfEventsExpected = 3;
        var memberIdExpected = new Guid("51ae7aca-2bb8-421a-a923-2ba2eb94bb3a");
        var eventTypeExpected = EventType.MEMBER_UNLOCKED;
        var entityTypeExpected = EntityType.MEMBER;
        var eventDataTypeExpected = typeof(MemberUnlockedEvent);

        // When
        var response = await HttpClient.DeleteAsync($"{BaseUrl}/{memberIdExpected.ToString()}/lock");

        // Then
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);
        var actualId = JsonConvert.DeserializeObject<Guid>(responseContent);
        Assert.That(actualId, Is.EqualTo(memberIdExpected));

        var storedEvents =
            await EventRepository.GetEventsForEntity<IMemberDomainEvent>(memberIdExpected, EntityType.MEMBER);
        Assert.That(storedEvents, Has.Count.EqualTo(numberOfEventsExpected));

        var storedEvent = storedEvents[numberOfEventsExpected - 1];
        Assert.Multiple(() =>
        {
            Assert.That(storedEvent.EventType, Is.EqualTo(eventTypeExpected));
            Assert.That(storedEvent.EntityType, Is.EqualTo(entityTypeExpected));
            Assert.That(storedEvent.EntityId, Is.EqualTo(memberIdExpected));
            Assert.That(storedEvent.EventData.GetType(), Is.EqualTo(eventDataTypeExpected));
        });
    }

    [Test]
    public async Task GivenMemberId_WhenDeleteMember_ThenMemberDeletedEventExistsInRepositoryAndIdIsReturned()
    {
        // Given
        var numberOfEventsExpected = 2;
        var memberIdExpected = new Guid("60831440-06d2-4017-9a7b-016e9cd0b2dc");
        var eventTypeExpected = EventType.MEMBER_DELETED;
        var entityTypeExpected = EntityType.MEMBER;
        var eventDataTypeExpected = typeof(MemberDeletedEvent);
        
        var adminIdExpected = new Guid("1dd88382-f781-4bf8-94e3-05e99d1434fe");
        var tennisClubIdExpected = new TennisClubId(new Guid("1fc64a89-9e63-4e9f-96f7-e2120f0ca6c3"));
        var jwtToken = JwtTokenHelper.GenerateJwtToken(adminIdExpected.ToString(), tennisClubIdExpected.Id.ToString(), ["ADMIN"]);

        // When
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        var response = await HttpClient.DeleteAsync($"{BaseUrl}/{memberIdExpected.ToString()}");

        // Then
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);
        var actualId = JsonConvert.DeserializeObject<Guid>(responseContent);
        Assert.That(actualId, Is.EqualTo(memberIdExpected));

        var storedEvents =
            await EventRepository.GetEventsForEntity<IMemberDomainEvent>(memberIdExpected, EntityType.MEMBER);
        Assert.That(storedEvents, Has.Count.EqualTo(numberOfEventsExpected));

        var storedEvent = storedEvents[numberOfEventsExpected - 1];
        Assert.Multiple(() =>
        {
            Assert.That(storedEvent.EventType, Is.EqualTo(eventTypeExpected));
            Assert.That(storedEvent.EntityType, Is.EqualTo(entityTypeExpected));
            Assert.That(storedEvent.EntityId, Is.EqualTo(memberIdExpected));
            Assert.That(storedEvent.EventData.GetType(), Is.EqualTo(eventDataTypeExpected));
        });
    }

    [Test]
    public async Task GivenDeletedMemberId_WhenDeleteMemberAgain_ThenErrorResponseIsReturned()
    {
        // Given
        var deletedMemberId = new Guid("e8a2cd4c-69ad-4cf2-bca6-a60d88be6649");
        
        var adminIdExpected = new Guid("1dd88382-f781-4bf8-94e3-05e99d1434fe");
        var tennisClubIdExpected = new TennisClubId(new Guid("1fc64a89-9e63-4e9f-96f7-e2120f0ca6c3"));
        var jwtToken = JwtTokenHelper.GenerateJwtToken(adminIdExpected.ToString(), tennisClubIdExpected.Id.ToString(), ["ADMIN"]);
        
        // When
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        var response = await HttpClient.DeleteAsync($"{BaseUrl}/{deletedMemberId.ToString()}");

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);
        Assert.That(responseContent, Does.Contain("Member is already deleted!"));
    }

    [Test]
    public async Task GivenDeletedMemberId_WhenLockMember_ThenErrorResponseIsReturned()
    {
        // Given
        var deletedMemberId = new Guid("e8a2cd4c-69ad-4cf2-bca6-a60d88be6649");

        // When
        var response = await HttpClient.PostAsync($"{BaseUrl}/{deletedMemberId.ToString()}/lock", null);

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);
        Assert.That(responseContent, Does.Contain("Member is already deleted!"));
    }

    [Test]
    public async Task GivenDeletedMemberId_WhenUnlockMember_ThenErrorResponseIsReturned()
    {
        // Given
        var deletedMemberId = new Guid("e8a2cd4c-69ad-4cf2-bca6-a60d88be6649");

        // When
        var response = await HttpClient.DeleteAsync($"{BaseUrl}/{deletedMemberId.ToString()}/lock");

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);
        Assert.That(responseContent, Does.Contain("Member is already deleted!"));
    }

    [Test]
    public async Task GivenUpdateMemberCommand_WhenUpdateMember_ThenMemberFullNameChangedEventExistsInRepository()
    {
        // Given
        var numberOfEventsExpected = 2;
        var memberIdExpected = new Guid("60831440-06d2-4017-9a7b-016e9cd0b2dc");
        var eventTypeExpected = EventType.MEMBER_FULL_NAME_CHANGED;
        var entityTypeExpected = EntityType.MEMBER;
        var eventDataTypeExpected = typeof(MemberFullNameChangedEvent);
        var nameExpected = new FullName("Jane", "Doe");
        var updateMemberCommand = new MemberUpdateCommand(nameExpected.FirstName, nameExpected.LastName, null);
        
        var tennisClubIdExpected = new TennisClubId(new Guid("1fc64a89-9e63-4e9f-96f7-e2120f0ca6c3"));
        var jwtToken = JwtTokenHelper.GenerateJwtToken(memberIdExpected.ToString(), tennisClubIdExpected.Id.ToString(), ["MEMBER"]);
        
        var httpContent = new StringContent(JsonConvert.SerializeObject(updateMemberCommand), Encoding.UTF8,
            "application/json");

        // When
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        var response = await HttpClient.PatchAsync($"{BaseUrl}/{memberIdExpected.ToString()}", httpContent);

        // Then
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);

        var storedEvents =
            await EventRepository.GetEventsForEntity<IMemberDomainEvent>(memberIdExpected, EntityType.MEMBER);
        Assert.That(storedEvents, Has.Count.EqualTo(numberOfEventsExpected));

        var storedEvent = storedEvents[numberOfEventsExpected - 1];
        Assert.Multiple(() =>
        {
            Assert.That(storedEvent.EventType, Is.EqualTo(eventTypeExpected));
            Assert.That(storedEvent.EntityType, Is.EqualTo(entityTypeExpected));
            Assert.That(storedEvent.EntityId, Is.EqualTo(memberIdExpected));
            Assert.That(storedEvent.EventData.GetType(), Is.EqualTo(eventDataTypeExpected));
        });
    }

    [Test]
    public async Task GivenUpdateMemberCommand_WhenOnlyFirstNameInUpdateMemberCommand_ThenErrorResponseIsReturned()
    {
        // Given
        var memberId = new Guid("60831440-06d2-4017-9a7b-016e9cd0b2dc");
        var updateMemberCommand = new MemberUpdateCommand("Jane", null, null);
        var httpContent = new StringContent(JsonConvert.SerializeObject(updateMemberCommand), Encoding.UTF8,
            "application/json");
        
        var tennisClubIdExpected = new TennisClubId(new Guid("1fc64a89-9e63-4e9f-96f7-e2120f0ca6c3"));
        var jwtToken = JwtTokenHelper.GenerateJwtToken(memberId.ToString(), tennisClubIdExpected.Id.ToString(), ["MEMBER"]);

        // When
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        var response = await HttpClient.PatchAsync($"{BaseUrl}/{memberId.ToString()}", httpContent);

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);
        Assert.That(responseContent,
            Does.Contain("You have to provide either first and last name or an e-mail address!"));
    }

    [Test]
    public async Task GivenUpdateMemberCommand_WhenUpdateMember_ThenMemberEmailChangedEventExistsInRepository()
    {
        // Given
        var numberOfEventsExpected = 2;
        var memberIdExpected = new Guid("60831440-06d2-4017-9a7b-016e9cd0b2dc");
        var eventTypeExpected = EventType.MEMBER_EMAIL_CHANGED;
        var entityTypeExpected = EntityType.MEMBER;
        var eventDataTypeExpected = typeof(MemberEmailChangedEvent);
        var updateMemberCommand = new MemberUpdateCommand(null, null, "armin.otter@fhv.gorillaKaefig");
        var httpContent = new StringContent(JsonConvert.SerializeObject(updateMemberCommand), Encoding.UTF8,
            "application/json");
        
        var tennisClubIdExpected = new TennisClubId(new Guid("1fc64a89-9e63-4e9f-96f7-e2120f0ca6c3"));
        var jwtToken = JwtTokenHelper.GenerateJwtToken(memberIdExpected.ToString(), tennisClubIdExpected.Id.ToString(), ["MEMBER"]);
        
        // When
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        var response = await HttpClient.PatchAsync($"{BaseUrl}/{memberIdExpected.ToString()}", httpContent);

        // Then
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);

        var storedEvents =
            await EventRepository.GetEventsForEntity<IMemberDomainEvent>(memberIdExpected, EntityType.MEMBER);
        Assert.That(storedEvents, Has.Count.EqualTo(numberOfEventsExpected));

        var storedEvent = storedEvents[numberOfEventsExpected - 1];
        Assert.Multiple(() =>
        {
            Assert.That(storedEvent.EventType, Is.EqualTo(eventTypeExpected));
            Assert.That(storedEvent.EntityType, Is.EqualTo(entityTypeExpected));
            Assert.That(storedEvent.EntityId, Is.EqualTo(memberIdExpected));
            Assert.That(storedEvent.EventData.GetType(), Is.EqualTo(eventDataTypeExpected));
        });
    }

    [Test]
    public async Task
        GivenUpdateMemberCommand_WhenUpdateMember_ThenMemberFullNameChangedEventAndMemberEmailChangedEventExistsInRepository()
    {
        // Given
        var numberOfEventsExpected = 3;
        var memberIdExpected = new Guid("60831440-06d2-4017-9a7b-016e9cd0b2dc");

        var firstEventTypeExpected = EventType.MEMBER_FULL_NAME_CHANGED;
        var firstEntityTypeExpected = EntityType.MEMBER;
        var firstEventDataTypeExpected = typeof(MemberFullNameChangedEvent);

        var secondEventTypeExpected = EventType.MEMBER_EMAIL_CHANGED;
        var secondEntityTypeExpected = EntityType.MEMBER;
        var secondEventDataTypeExpected = typeof(MemberEmailChangedEvent);

        var name = new FullName("Jane", "Doe");
        var updateMemberCommand =
            new MemberUpdateCommand(name.FirstName, name.LastName, "armin.otter@fhv.gorillaKaefig");
        var httpContent = new StringContent(JsonConvert.SerializeObject(updateMemberCommand), Encoding.UTF8,
            "application/json");
        
        var tennisClubIdExpected = new TennisClubId(new Guid("1fc64a89-9e63-4e9f-96f7-e2120f0ca6c3"));
        var jwtToken = JwtTokenHelper.GenerateJwtToken(memberIdExpected.ToString(), tennisClubIdExpected.Id.ToString(), ["MEMBER"]);

        // When
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        var response = await HttpClient.PatchAsync($"{BaseUrl}/{memberIdExpected.ToString()}", httpContent);

        // Then
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);

        var storedEvents =
            await EventRepository.GetEventsForEntity<IMemberDomainEvent>(memberIdExpected, EntityType.MEMBER);
        Assert.That(storedEvents, Has.Count.EqualTo(numberOfEventsExpected));

        var firstStoredEvent = storedEvents[numberOfEventsExpected - 2];
        Assert.Multiple(() =>
        {
            Assert.That(firstStoredEvent.EventType, Is.EqualTo(firstEventTypeExpected));
            Assert.That(firstStoredEvent.EntityType, Is.EqualTo(firstEntityTypeExpected));
            Assert.That(firstStoredEvent.EntityId, Is.EqualTo(memberIdExpected));
            Assert.That(firstStoredEvent.EventData.GetType(), Is.EqualTo(firstEventDataTypeExpected));
        });

        var secondStoredEvent = storedEvents[numberOfEventsExpected - 1];
        Assert.Multiple(() =>
        {
            Assert.That(secondStoredEvent.EventType, Is.EqualTo(secondEventTypeExpected));
            Assert.That(secondStoredEvent.EntityType, Is.EqualTo(secondEntityTypeExpected));
            Assert.That(secondStoredEvent.EntityId, Is.EqualTo(memberIdExpected));
            Assert.That(secondStoredEvent.EventData.GetType(), Is.EqualTo(secondEventDataTypeExpected));
        });
    }

    [Test]
    public async Task GivenUpdateMemberCommand_WhenEmptyUpdateMemberCommand_ThenErrorResponseIsReturned()
    {
        // Given
        var memberId = new Guid("60831440-06d2-4017-9a7b-016e9cd0b2dc");
        var updateMemberCommand = new MemberUpdateCommand(null, null, null);
        var httpContent = new StringContent(JsonConvert.SerializeObject(updateMemberCommand), Encoding.UTF8,
            "application/json");
        
        var tennisClubIdExpected = new TennisClubId(new Guid("1fc64a89-9e63-4e9f-96f7-e2120f0ca6c3"));
        var jwtToken = JwtTokenHelper.GenerateJwtToken(memberId.ToString(), tennisClubIdExpected.Id.ToString(), ["MEMBER"]);
        
        // When
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        var response = await HttpClient.PatchAsync($"{BaseUrl}/{memberId.ToString()}", httpContent);

        // Then
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);
        Assert.That(responseContent,
            Does.Contain("You have to provide either first and last name or an e-mail address!"));
    }
}