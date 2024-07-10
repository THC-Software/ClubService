using ClubService.Domain.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace ClubService.IntegrationTests;

public class WebAppFactory(
    int postgresPort,
    Mock<ITennisClubReadModelRepository> mockTennisClubReadModelRepository,
    Mock<ISubscriptionTierReadModelRepository> mockSubscriptionTierReadModelRepository,
    Mock<IAdminReadModelRepository> mockAdminReadModelRepository,
    Mock<IMemberReadModelRepository> mockMemberReadModelRepository,
    Mock<ILoginRepository> mockLoginRepository) : WebApplicationFactory<Program>
{
    private readonly Dictionary<string, string> _testAppSettings = new()
    {
        ["ConnectionStrings:event-store-connection"] =
            $"Host=localhost:{postgresPort};Username=user;Password=password;Database=club-service-event-store",
        ["ConnectionStrings:read-store-connection"] =
            $"Host=localhost:{postgresPort};Username=user;Password=password;Database=club-service-read-store",
        ["ConnectionStrings:login-store-connection"] =
            $"Host=localhost:{postgresPort};Username=user;Password=password;Database=club-service-login-store",

        // Redis Configuration
        ["RedisConfiguration:Host"] = "localhost:6379",
        ["RedisConfiguration:PollingIntervalMilliSeconds"] = "1000",
        ["RedisConfiguration:Streams:0:StreamName"] = "club_service_events.public.DomainEvent",
        ["RedisConfiguration:Streams:0:ConsumerGroup"] = "club_service_events.domain.events.group",
        ["RedisConfiguration:Streams:1:StreamName"] = "tournament_service_events.public.DomainEvent",
        ["RedisConfiguration:Streams:1:ConsumerGroup"] = "tournament_service_events.domain.events.group",

        // SMTP Configuration
        ["SmtpConfiguration:Host"] = "localhost",
        ["SmtpConfiguration:Port"] = "1025",
        ["SmtpConfiguration:SenderEmailAddress"] = "admin@thcdornbirn.at",
        ["SmtpConfiguration:PollingIntervalMilliSeconds"] = "10000"
    };

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ConfigureAppSettings(builder);
        RegisterServices(builder);
        base.ConfigureWebHost(builder);

        builder.UseEnvironment("Test");
    }

    private void ConfigureAppSettings(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(configBuilder => { configBuilder.AddInMemoryCollection(_testAppSettings!); });
    }

    private void RegisterServices(IWebHostBuilder builder)
    {
        // mock repositories for write side integration tests
        builder.ConfigureServices(services =>
        {
            services.AddScoped(_ => mockTennisClubReadModelRepository.Object);
            services.AddScoped(_ => mockSubscriptionTierReadModelRepository.Object);
            services.AddScoped(_ => mockAdminReadModelRepository.Object);
            services.AddScoped(_ => mockMemberReadModelRepository.Object);
            services.AddScoped(_ => mockLoginRepository.Object);
        });
    }
}