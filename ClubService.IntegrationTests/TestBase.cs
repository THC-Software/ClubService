using ClubService.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace ClubService.IntegrationTests;

public class TestBase : WebApplicationFactory<Program>
{
    private ApplicationDbContext _dbContext;
    private WebApplicationFactory<Program> _factory;
    private PostgreSqlContainer _postgresContainer;
    protected HttpClient HttpClient;
    
    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");
        
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("debezium/postgres:16-alpine")
            .WithUsername("user")
            .WithPassword("password")
            .WithDatabase("club-service-test")
            .WithPortBinding(35053, 5432)
            .Build();
        
        await _postgresContainer.StartAsync();
        
        _factory = new WebApplicationFactory<Program>();
        HttpClient = _factory.CreateClient();
        
        var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>() ??
                           throw new Exception("Scope factory not found");
        var scope = scopeFactory.CreateScope() ??
                    throw new Exception("Could not create Scope");
        _dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>() ??
                     throw new Exception("Could not get ApplicationDbContext");
    }
    
    [SetUp]
    public async Task Setup()
    {
        await _dbContext.Database.EnsureCreatedAsync();
    }
    
    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        HttpClient.Dispose();
        await _factory.DisposeAsync();
        await _dbContext.DisposeAsync();
        await _postgresContainer.StopAsync();
    }
    
    [TearDown]
    public async Task TearDown()
    {
        await _dbContext.Database.EnsureDeletedAsync();
    }
}