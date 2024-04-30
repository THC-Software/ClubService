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
    [Test]
    public async Task GivenValidInputs_WhenRegisterTennisClub_ThenReturnsClubId()
    {
        // Given
        var tennisClubRegisterCommand =
            new TennisClubRegisterCommand("Test Tennis Club", Guid.NewGuid().ToString());

        var eventRepositoryMock = new Mock<IEventRepository>();
        eventRepositoryMock.Setup(repo => repo.Save(It.IsAny<DomainEnvelope<ITennisClubDomainEvent>>()))
            .Returns(Task.CompletedTask);

        var tennisClubService = new RegisterTennisClubService(eventRepositoryMock.Object);

        // When
        _ = await tennisClubService.RegisterTennisClub(tennisClubRegisterCommand);

        // Then
        eventRepositoryMock.Verify(repo => repo.Save(It.IsAny<DomainEnvelope<ITennisClubDomainEvent>>()),
            Times.Exactly(1));
        eventRepositoryMock.Verify(repo => repo.Save(It.Is<DomainEnvelope<ITennisClubDomainEvent>>(e =>
            e.EventType == EventType.TENNIS_CLUB_REGISTERED &&
            e.EntityType == EntityType.TENNIS_CLUB &&
            e.EventData.GetType() == typeof(TennisClubRegisteredEvent))), Times.Once);
    }
}