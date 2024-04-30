using ClubService.Application.Impl;
using ClubService.Domain.Event;
using ClubService.Domain.Event.TennisClub;
using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.Repository;
using Moq;

namespace ClubService.Application.UnitTests;

[TestFixture]
public class UpdateTennisClubServiceTests
{
    [SetUp]
    public void Setup()
    {
        _eventRepositoryMock = new Mock<IEventRepository>();
        _updateTennisClubService = new UpdateTennisClubService(_eventRepositoryMock.Object);
    }
    
    private UpdateTennisClubService _updateTennisClubService;
    private Mock<IEventRepository> _eventRepositoryMock;
    
    [Test]
    public async Task GivenExistingTennisClub_WhenLockTennisClub_ThenRepoIsCalledWithExpectedEvent()
    {
        // Given
        var tennisClubId = new TennisClubId(Guid.NewGuid());
        var name = "Test Tennis Club";
        var isLocked = false;
        var subscriptionTierId = new SubscriptionTierId(Guid.NewGuid());
        List<MemberId> memberIds = [];
        
        var tennisClubRegisteredEvent =
            new TennisClubRegisteredEvent(tennisClubId, name, isLocked,
                subscriptionTierId, memberIds);
        
        var domainEnvelope =
            new DomainEnvelope<ITennisClubDomainEvent>(Guid.NewGuid(), tennisClubId.Id,
                EventType.TENNIS_CLUB_REGISTERED, EntityType.TENNIS_CLUB, DateTime.UtcNow, tennisClubRegisteredEvent);
        var existingDomainEvents = new List<DomainEnvelope<ITennisClubDomainEvent>>
        {
            domainEnvelope
        };
        
        _eventRepositoryMock.Setup(repo => repo.GetEventsForEntity<ITennisClubDomainEvent>(It.IsAny<Guid>()))
            .Returns(existingDomainEvents);
        
        // When
        _ = await _updateTennisClubService.LockTennisClub(tennisClubId.Id.ToString());
        
        // Then
        _eventRepositoryMock.Verify(repo => repo.Save(It.Is<DomainEnvelope<ITennisClubDomainEvent>>(e =>
            e.EventType == EventType.TENNIS_CLUB_LOCKED &&
            e.EntityType == EntityType.TENNIS_CLUB &&
            e.EventData.GetType() == typeof(TennisClubLockedEvent))), Times.Once);
    }
    
    [Test]
    public void GivenNonExistentTennisClubId_WhenLockTennisClub_ThenExceptionIsThrown()
    {
        // Given
        var clubId = Guid.NewGuid().ToString();
        _eventRepositoryMock.Setup(repo => repo.GetEventsForEntity<ITennisClubDomainEvent>(It.IsAny<Guid>()))
            .Returns(new List<DomainEnvelope<ITennisClubDomainEvent>>());
        
        // When ... Then
        Assert.ThrowsAsync<ArgumentException>(() => _updateTennisClubService.LockTennisClub(clubId));
    }
}