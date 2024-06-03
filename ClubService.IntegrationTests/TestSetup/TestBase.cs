using ClubService.Domain.Event.SubscriptionTier;
using ClubService.Domain.Event.TennisClub;
using ClubService.Domain.Model.Enum;
using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.ReadModel;
using ClubService.Domain.Repository;
using ClubService.Infrastructure.DbContexts;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Testcontainers.PostgreSql;

namespace ClubService.IntegrationTests.TestSetup;

public class TestBase
{
    private EventStoreDbContext _eventStoreDbContext;
    private WebAppFactory _factory;
    private PostgreSqlContainer _postgresContainer;
    protected IEventRepository EventRepository;
    protected HttpClient HttpClient;
    private Mock<ITennisClubReadModelRepository> _mockTennisClubReadModelRepository;
    private Mock<ISubscriptionTierReadModelRepository> _mockSubscriptionTierReadModelRepository;
    
    [SetUp]
    public async Task Setup()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");
        
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("debezium/postgres:16-alpine")
            .WithUsername("user")
            .WithPassword("password")
            .WithDatabase("club-service-test")
            .WithPortBinding(5432, true)
            .Build();
        
        await _postgresContainer.StartAsync();
        
        // these two mock repositories are needed for the write side RegisterMember integration test 
        _mockTennisClubReadModelRepository = new Mock<ITennisClubReadModelRepository>();
        _mockSubscriptionTierReadModelRepository = new Mock<ISubscriptionTierReadModelRepository>();
        
        _factory = new WebAppFactory(
            _postgresContainer.GetConnectionString(),
            _mockTennisClubReadModelRepository,
            _mockSubscriptionTierReadModelRepository
        );
        HttpClient = _factory.CreateClient();
        
        var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>() ??
                           throw new Exception("Scope factory not found");
        var scope = scopeFactory.CreateScope() ??
                    throw new Exception("Could not create Scope");
        
        _eventStoreDbContext = scope.ServiceProvider.GetService<EventStoreDbContext>() ??
                               throw new Exception("Could not get ApplicationDbContext");
        EventRepository = scope.ServiceProvider.GetService<IEventRepository>() ??
                          throw new Exception("Could not get EventRepository");
        await _eventStoreDbContext.Database.EnsureCreatedAsync();
        
        // tennisClub and subscriptionTier mock
        var tennisClubRegisteredEvent = new TennisClubRegisteredEvent(
            new TennisClubId(Guid.NewGuid()),
            "Sample Tennis Club",
            new SubscriptionTierId(Guid.NewGuid()),
            TennisClubStatus.ACTIVE
        );
        var tennisClubReadModel = TennisClubReadModel.FromDomainEvent(tennisClubRegisteredEvent);
        _mockTennisClubReadModelRepository
            .Setup(repo => repo.GetTennisClubById(It.IsAny<Guid>()))
            .ReturnsAsync(tennisClubReadModel);
        
        var subscriptionTierCreatedEvent = new SubscriptionTierCreatedEvent(
            new SubscriptionTierId(Guid.NewGuid()),
            "Standard",
            10
        );
        var subscriptionTierReadModel = SubscriptionTierReadModel.FromDomainEvent(subscriptionTierCreatedEvent);
        _mockSubscriptionTierReadModelRepository
            .Setup(repo => repo.GetSubscriptionTierById(It.IsAny<Guid>()))
            .ReturnsAsync(subscriptionTierReadModel);
    }
    
    [TearDown]
    public async Task TearDown()
    {
        HttpClient.Dispose();
        await _factory.DisposeAsync();
        await _eventStoreDbContext.DisposeAsync();
        await _postgresContainer.StopAsync();
    }
}