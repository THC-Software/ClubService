using ClubService.Domain.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace ClubService.IntegrationTests;

public class WebAppFactory(
    string connectionString,
    Mock<ITennisClubReadModelRepository> mockTennisClubReadModelRepository,
    Mock<ISubscriptionTierReadModelRepository> mockSubscriptionTierReadModelRepository,
    Mock<IAdminReadModelRepository> mockAdminReadModelRepository,
    Mock<IMemberReadModelRepository> mockMemberReadModelRepository) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        ConfigureAppSettings(builder);
        RegisterServices(builder);
    }

    private void ConfigureAppSettings(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(configBuilder =>
        {
            configBuilder.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["ConnectionStrings:event-store-connection"] = connectionString,

                // Redis Configuration
                ["RedisConfiguration:Host"] = "localhost:6379",
                ["RedisConfiguration:Streams:0:StreamName"] = "club_service_events.public.DomainEvent",
                ["RedisConfiguration:Streams:0:ConsumerGroup"] = "club_service_events.domain.events.group",
                ["RedisConfiguration:Streams:1:StreamName"] = "tournament_service_events.public.DomainEvent",
                ["RedisConfiguration:Streams:1:ConsumerGroup"] = "tournament_service_events.domain.events.group",

                // SMTP Configuration
                ["SmtpConfiguration:Host"] = "localhost",
                ["SmtpConfiguration:Port"] = "1025"
            }!);
        });
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
        });
    }
}