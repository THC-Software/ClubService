using ClubService.Application.Api.Exceptions;
using ClubService.Application.Commands;
using ClubService.Application.Impl;
using ClubService.Domain.Event;
using ClubService.Domain.Event.SubscriptionTier;
using ClubService.Domain.Event.TennisClub;
using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.Repository;
using Moq;

namespace ClubService.Application.UnitTests;

[TestFixture]
public class RegisterTennisClubServiceTests
{
    [SetUp]
    public void Setup()
    {
        _eventRepositoryMock = new Mock<IEventRepository>();
        _registerTennisClubService = new RegisterTennisClubService(_eventRepositoryMock.Object);
    }
    
    private RegisterTennisClubService _registerTennisClubService;
    private Mock<IEventRepository> _eventRepositoryMock;
    
    [Test]
    public async Task GivenValidInputs_WhenRegisterTennisClub_ThenRepoIsCalledWithExpectedEvent()
    {
        // Given
        var subscriptionTierId = Guid.NewGuid();
        var tennisClubRegisterCommand =
            new TennisClubRegisterCommand("Test Tennis Club", subscriptionTierId.ToString());
        List<DomainEnvelope<ISubscriptionTierDomainEvent>> subscriptionTierDomainEvents =
        [
            new DomainEnvelope<ISubscriptionTierDomainEvent>(
                new Guid("8d4d3eff-b77b-4e21-963b-e211366bb94b"),
                subscriptionTierId,
                EventType.SUBSCRIPTION_TIER_CREATED,
                EntityType.SUBSCRIPTION_TIER,
                DateTime.UtcNow,
                new SubscriptionTierCreatedEvent(
                    new SubscriptionTierId(subscriptionTierId),
                    "Gold Subscription Tier",
                    200)
            )
        ];
        
        _eventRepositoryMock.Setup(repo => repo.GetEventsForEntity<ISubscriptionTierDomainEvent>(subscriptionTierId))
            .ReturnsAsync(subscriptionTierDomainEvents);
        _eventRepositoryMock.Setup(repo => repo.Save(It.IsAny<DomainEnvelope<ITennisClubDomainEvent>>()))
            .Returns(Task.CompletedTask);
        
        // When
        _ = await _registerTennisClubService.RegisterTennisClub(tennisClubRegisterCommand);
        
        // Then
        _eventRepositoryMock.Verify(repo => repo.Save(It.Is<DomainEnvelope<ITennisClubDomainEvent>>(e =>
            e.EventType == EventType.TENNIS_CLUB_REGISTERED &&
            e.EntityType == EntityType.TENNIS_CLUB &&
            e.EventData.GetType() == typeof(TennisClubRegisteredEvent))), Times.Once);
    }
    
    [Test]
    public void GivenNonExistentSubscriptionTierId_WhenRegisterTennisClub_ThenExceptionIsThrown()
    {
        // Given
        var subscriptionTierId = Guid.NewGuid();
        var tennisClubRegisterCommand =
            new TennisClubRegisterCommand("Test Tennis Club", subscriptionTierId.ToString());
        List<DomainEnvelope<ISubscriptionTierDomainEvent>> subscriptionTierDomainEvents = [];
        
        _eventRepositoryMock.Setup(repo => repo.GetEventsForEntity<ISubscriptionTierDomainEvent>(subscriptionTierId))
            .ReturnsAsync(subscriptionTierDomainEvents);
        
        // When ... Then
        Assert.ThrowsAsync<SubscriptionTierNotFoundException>(async () =>
            await _registerTennisClubService.RegisterTennisClub(tennisClubRegisterCommand));
    }
}