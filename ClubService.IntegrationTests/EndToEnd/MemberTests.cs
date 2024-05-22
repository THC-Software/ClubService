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
        
        var storedEvents = EventRepository.GetEventsForEntity<IMemberDomainEvent>(Guid.Parse(responseContent));
        Assert.That(storedEvents, Has.Count.EqualTo(numberOfEventsExpected));
        
        var storedEvent = storedEvents[0];
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
}