using System.Net;
using System.Text;
using ClubService.Application.Commands;
using ClubService.Domain.Event;
using ClubService.Domain.Event.Member;
using ClubService.Domain.Model.Enum;
using ClubService.Domain.Model.ValueObject;
using ClubService.IntegrationTests.TestSetup;
using Newtonsoft.Json;

namespace ClubService.IntegrationTests.EndToEnd;

[TestFixture]
public class MemberTests : TestBase
{
    private const string BaseUrl = "/api/v1.0/members";
    
    [Test]
    public async Task
        GivenRegisterMemberCommand_WhenRegisterMember_ThenMemberRegisteredEventExistsInRepository()
    {
        // Given
        var numberOfEventsExpected = 1;
        var eventTypeExpected = EventType.MEMBER_REGISTERED;
        var entityTypeExpected = EntityType.MEMBER;
        var eventDataTypeExpected = typeof(MemberRegisteredEvent);
        var nameExpected = new FullName("John", "Doe");
        var statusExpected = MemberStatus.NONE;
        var tennisClubIdExpected = new TennisClubId(new Guid("1fc64a89-9e63-4e9f-96f7-e2120f0ca6c3"));
        var registerMemberCommand = new MemberRegisterCommand(nameExpected.FirstName, nameExpected.LastName,
            "john.doe@dev.com", tennisClubIdExpected.Id.ToString());
        var httpContent = new StringContent(JsonConvert.SerializeObject(registerMemberCommand), Encoding.UTF8,
            "application/json");
        
        // When
        var response = await HttpClient.PostAsync(BaseUrl, httpContent);
        
        // Then
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);
        
        var storedEvents = await EventRepository.GetEventsForEntity<IMemberDomainEvent>(Guid.Parse(responseContent));
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
        
        // When
        var response = await HttpClient.PostAsync($"{BaseUrl}/{memberIdExpected.ToString()}/lock", null);
        
        // Then
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);
        Assert.That(responseContent, Is.EqualTo(memberIdExpected.ToString()));
        
        var storedEvents = await EventRepository.GetEventsForEntity<IMemberDomainEvent>(memberIdExpected);
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
        Assert.That(responseContent, Is.EqualTo(memberIdExpected.ToString()));
        
        var storedEvents = await EventRepository.GetEventsForEntity<IMemberDomainEvent>(memberIdExpected);
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
        
        // When
        var response = await HttpClient.DeleteAsync($"{BaseUrl}/{memberIdExpected.ToString()}");
        
        // Then
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.That(responseContent, Is.Not.Null);
        Assert.That(responseContent, Is.EqualTo(memberIdExpected.ToString()));
        
        var storedEvents = EventRepository.GetEventsForEntity<IMemberDomainEvent>(memberIdExpected);
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
        
        // When
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
}