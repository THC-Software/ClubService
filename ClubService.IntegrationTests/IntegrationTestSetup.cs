using Testcontainers.PostgreSql;

namespace ClubService.IntegrationTests;

[SetUpFixture]
public static class IntegrationTestSetup
{
    public static PostgreSqlContainer PostgresContainer { get; private set; }

    [OneTimeSetUp]
    public static async Task OneTimeSetup()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Test");

        PostgresContainer = new PostgreSqlBuilder()
            .WithImage("debezium/postgres:16-alpine")
            .WithUsername("user")
            .WithPassword("password")
            .WithDatabase("club-service-test")
            .WithPortBinding(5432, true)
            .Build();

        await PostgresContainer.StartAsync();
    }

    [OneTimeTearDown]
    public static async Task OneTimeTearDown()
    {
        await PostgresContainer.StopAsync();
    }
}