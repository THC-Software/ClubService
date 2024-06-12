using ClubService.Domain.Repository;
using ClubService.Infrastructure.DbContexts;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace ClubService.IntegrationTests;

public class TestBase
{
    private EventStoreDbContext _eventStoreDbContext;
    private WebAppFactory _factory;
    protected IEventRepository EventRepository;
    protected HttpClient HttpClient;
    protected Mock<IAdminReadModelRepository> MockAdminReadModelRepository;
    protected Mock<IMemberReadModelRepository> MockMemberReadModelRepository;
    protected Mock<ISubscriptionTierReadModelRepository> MockSubscriptionTierReadModelRepository;
    protected Mock<ITennisClubReadModelRepository> MockTennisClubReadModelRepository;

    [SetUp]
    public async Task Setup()
    {
        // mock repositories for write side integration tests
        MockTennisClubReadModelRepository = new Mock<ITennisClubReadModelRepository>();
        MockSubscriptionTierReadModelRepository = new Mock<ISubscriptionTierReadModelRepository>();
        MockAdminReadModelRepository = new Mock<IAdminReadModelRepository>();
        MockMemberReadModelRepository = new Mock<IMemberReadModelRepository>();

        _factory = new WebAppFactory(
            IntegrationTestSetup.PostgresContainer.GetConnectionString(),
            MockTennisClubReadModelRepository,
            MockSubscriptionTierReadModelRepository,
            MockAdminReadModelRepository,
            MockMemberReadModelRepository
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
    }

    [TearDown]
    public async Task TearDown()
    {
        await _eventStoreDbContext.Database.EnsureDeletedAsync();
        HttpClient.Dispose();
        await _factory.DisposeAsync();
        await _eventStoreDbContext.DisposeAsync();
    }
}