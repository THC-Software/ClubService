using ClubService.Application.Commands;
using ClubService.Application.Impl;
using ClubService.Domain.Event;
using ClubService.Domain.Event.TennisClub;
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
        var tennisClubRegisterCommand =
            new TennisClubRegisterCommand("Test Tennis Club", Guid.NewGuid().ToString());
        
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
}