using ClubService.Application.Api.Exceptions;
using ClubService.Application.Commands;
using ClubService.Application.Impl;
using ClubService.Domain.Api;
using ClubService.Domain.Event;
using ClubService.Domain.Event.Admin;
using ClubService.Domain.Event.SubscriptionTier;
using ClubService.Domain.Event.TennisClub;
using ClubService.Domain.Model.Entity;
using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.Repository;
using ClubService.Domain.Repository.Transaction;
using Moq;

namespace ClubService.Application.UnitTests;

[TestFixture]
public class RegisterTennisClubServiceTests
{
    [SetUp]
    public void Setup()
    {
        _eventRepositoryMock = new Mock<IEventRepository>();
        _transactionManagerMock = new Mock<ITransactionManager>();
        _passwordHasherServiceMock = new Mock<IPasswordHasherService>();
        _loginRepositoryMock = new Mock<ILoginRepository>();
        _loggerMock = new Mock<ILoggerService<RegisterTennisClubService>>();

        // set up the TransactionScope method to call the passed function
        _transactionManagerMock
            .Setup(mgr => mgr.TransactionScope(It.IsAny<Func<Task>>()))
            .Returns((Func<Task> transactionalOperation) => transactionalOperation());

        _registerTennisClubService = new RegisterTennisClubService(
            _eventRepositoryMock.Object,
            _transactionManagerMock.Object,
            _passwordHasherServiceMock.Object,
            _loginRepositoryMock.Object,
            _loggerMock.Object
        );
    }

    private Mock<IEventRepository> _eventRepositoryMock;
    private Mock<ITransactionManager> _transactionManagerMock;
    private Mock<IPasswordHasherService> _passwordHasherServiceMock;
    private Mock<ILoginRepository> _loginRepositoryMock;
    private Mock<ILoggerService<RegisterTennisClubService>> _loggerMock;
    private RegisterTennisClubService _registerTennisClubService;

    [Test]
    public async Task GivenValidInputs_WhenRegisterTennisClub_ThenRepoIsCalledWithExpectedEvent()
    {
        // Given
        const int eventCountExpected = 0;
        const string adminUsername = "testuser";
        const string adminPassword = "test";
        const string adminFirstName = "John";
        const string adminLastName = "Doe";
        var subscriptionTierId = Guid.NewGuid();
        var tennisClubRegisterCommand = new TennisClubRegisterCommand("Test Tennis Club", subscriptionTierId,
            adminUsername, adminPassword, adminFirstName, adminLastName);
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

        _eventRepositoryMock.Setup(repo =>
                repo.GetEventsForEntity<ISubscriptionTierDomainEvent>(subscriptionTierId, EntityType.SUBSCRIPTION_TIER))
            .ReturnsAsync(subscriptionTierDomainEvents);
        _passwordHasherServiceMock.Setup(service => service.HashPassword(adminPassword)).Returns(adminPassword);

        // When
        _ = await _registerTennisClubService.RegisterTennisClub(tennisClubRegisterCommand);

        // Then
        _eventRepositoryMock.Verify(repo =>
                repo.Append(It.Is<DomainEnvelope<ITennisClubDomainEvent>>(e =>
                        e.EventType == EventType.TENNIS_CLUB_REGISTERED &&
                        e.EntityType == EntityType.TENNIS_CLUB &&
                        e.EventData.GetType() == typeof(TennisClubRegisteredEvent)),
                    eventCountExpected), Times.Once
        );

        _eventRepositoryMock.Verify(repo =>
                repo.Append(It.Is<DomainEnvelope<IAdminDomainEvent>>(e =>
                        e.EventType == EventType.ADMIN_REGISTERED &&
                        e.EntityType == EntityType.ADMIN &&
                        e.EventData.GetType() == typeof(AdminRegisteredEvent)),
                    eventCountExpected), Times.Once
        );

        _loginRepositoryMock.Verify(repo => repo.Add(It.Is<UserPassword>(up => up.HashedPassword == adminPassword)));
    }

    [Test]
    public void GivenNonExistentSubscriptionTierId_WhenRegisterTennisClub_ThenExceptionIsThrown()
    {
        // Given
        var subscriptionTierId = Guid.NewGuid();
        var tennisClubRegisterCommand =
            new TennisClubRegisterCommand("Test Tennis Club", subscriptionTierId, "testuser", "test", "John", "Doe");
        List<DomainEnvelope<ISubscriptionTierDomainEvent>> subscriptionTierDomainEvents = [];

        _eventRepositoryMock.Setup(repo =>
                repo.GetEventsForEntity<ISubscriptionTierDomainEvent>(subscriptionTierId, EntityType.SUBSCRIPTION_TIER))
            .ReturnsAsync(subscriptionTierDomainEvents);

        // When ... Then
        Assert.ThrowsAsync<SubscriptionTierNotFoundException>(async () =>
            await _registerTennisClubService.RegisterTennisClub(tennisClubRegisterCommand));
    }
}