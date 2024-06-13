using ClubService.Domain.Repository;
using ClubService.Infrastructure.DbContexts;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Testcontainers.PostgreSql;

namespace ClubService.IntegrationTests;

[SetUpFixture]
public static class IntegrationTestSetup
{
    private static PostgreSqlContainer _postgresContainer = null!;
    private static WebAppFactory _webAppFactory = null!;
    public static EventStoreDbContext EventStoreDbContext { get; private set; } = null!;
    public static IEventRepository EventRepository { get; private set; } = null!;
    public static HttpClient HttpClient { get; private set; } = null!;
    public static Mock<IAdminReadModelRepository> MockAdminReadModelRepository { get; private set; } = null!;
    public static Mock<IMemberReadModelRepository> MockMemberReadModelRepository { get; private set; } = null!;

    public static Mock<ISubscriptionTierReadModelRepository> MockSubscriptionTierReadModelRepository
    {
        get;
        private set;
    } = null!;

    public static Mock<ITennisClubReadModelRepository> MockTennisClubReadModelRepository { get; private set; } = null!;

    [OneTimeSetUp]
    public static async Task OneTimeSetup()
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

        // mock repositories for write side integration tests
        MockTennisClubReadModelRepository = new Mock<ITennisClubReadModelRepository>();
        MockSubscriptionTierReadModelRepository = new Mock<ISubscriptionTierReadModelRepository>();
        MockAdminReadModelRepository = new Mock<IAdminReadModelRepository>();
        MockMemberReadModelRepository = new Mock<IMemberReadModelRepository>();

        _webAppFactory = new WebAppFactory(
            _postgresContainer.GetConnectionString(),
            MockTennisClubReadModelRepository,
            MockSubscriptionTierReadModelRepository,
            MockAdminReadModelRepository,
            MockMemberReadModelRepository
        );

        HttpClient = _webAppFactory.CreateClient();

        var scopeFactory = _webAppFactory.Services.GetService<IServiceScopeFactory>() ??
                           throw new Exception("Scope factory not found");
        var scope = scopeFactory.CreateScope() ??
                    throw new Exception("Could not create Scope");

        EventStoreDbContext = scope.ServiceProvider.GetService<EventStoreDbContext>() ??
                              throw new Exception("Could not get ApplicationDbContext");
        EventRepository = scope.ServiceProvider.GetService<IEventRepository>() ??
                          throw new Exception("Could not get EventRepository");
    }

    [OneTimeTearDown]
    public static async Task OneTimeTearDown()
    {
        await EventStoreDbContext.DisposeAsync();
        await _postgresContainer.StopAsync();
        await _postgresContainer.DisposeAsync();
        HttpClient.Dispose();
        await _webAppFactory.DisposeAsync();
    }
}