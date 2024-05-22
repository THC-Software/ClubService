using ClubService.Domain.Repository;
using ClubService.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace ClubService.IntegrationTests.TestSetup;

public class TestBase
{
    private ApplicationDbContext _dbContext;
    private WebAppFactory _factory;
    private PostgreSqlContainer _postgresContainer;
    protected IEventRepository EventRepository;
    protected HttpClient HttpClient;
    
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
        
        _factory = new WebAppFactory(_postgresContainer.GetConnectionString());
        HttpClient = _factory.CreateClient();
        
        var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>() ??
                           throw new Exception("Scope factory not found");
        var scope = scopeFactory.CreateScope() ??
                    throw new Exception("Could not create Scope");
        
        _dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>() ??
                     throw new Exception("Could not get ApplicationDbContext");
        EventRepository = scope.ServiceProvider.GetService<IEventRepository>() ??
                          throw new Exception("Could not get EventRepository");
        await _dbContext.Database.EnsureCreatedAsync();
    }
    
    [TearDown]
    public async Task TearDown()
    {
        HttpClient.Dispose();
        await _factory.DisposeAsync();
        await _dbContext.DisposeAsync();
        await _postgresContainer.StopAsync();
    }
}