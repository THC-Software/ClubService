using ClubService.Domain.Event;
using ClubService.Domain.Event.Admin;
using ClubService.Domain.Model.Entity;
using ClubService.Domain.Model.ValueObject;

namespace ClubService.Domain.UnitTests;

[TestFixture]
public class AdminTests
{
    [Test]
    public void GivenValidInputs_WhenProcessAdminRegisterCommand_ThenReturnsDomainEnvelopeWithAdminRegisteredEvent()
    {
        // Given
        var eventTypeExpected = EventType.ADMIN_REGISTERED;
        var entityTypeExpected = EntityType.ADMIN;
        var usernameExpected = "admin01";
        var firstNameExpected = "John";
        var lastNameExpected = "Doe";
        var tennisClubIdExpected = "6a463e1a-6b0f-4825-83c3-911f12f80076";
        var admin = new Admin();
        
        // When
        var domainEnvelopes = admin.ProcessAdminRegisteredCommand(usernameExpected,
            new FullName(firstNameExpected, lastNameExpected), new TennisClubId(new Guid(tennisClubIdExpected)));
        
        // Then
        Assert.That(domainEnvelopes, Is.Not.Null);
        Assert.That(domainEnvelopes, Has.Count.EqualTo(1));
        
        var domainEnvelope = domainEnvelopes[0];
        Assert.Multiple(() =>
        {
            Assert.That(domainEnvelope.EntityId, Is.Not.Empty);
            Assert.That(domainEnvelope.EventType, Is.EqualTo(eventTypeExpected));
            Assert.That(domainEnvelope.EntityType, Is.EqualTo(entityTypeExpected));
        });
        
        var adminRegisteredEvent = domainEnvelope.EventData as AdminRegisteredEvent;
        Assert.That(adminRegisteredEvent, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(adminRegisteredEvent.AdminId.Id, Is.Not.Empty);
            Assert.That(adminRegisteredEvent.Username, Is.EqualTo(usernameExpected));
            Assert.That(adminRegisteredEvent.Name, Is.EqualTo(new FullName(firstNameExpected, lastNameExpected)));
            Assert.That(adminRegisteredEvent.TennisClubId, Is.EqualTo(new TennisClubId(new Guid(tennisClubIdExpected))));
        });
        
    }
}