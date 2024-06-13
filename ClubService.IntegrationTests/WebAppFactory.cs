using ClubService.Domain.Repository;
using ClubService.Infrastructure.DbContexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
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
        builder.ConfigureServices(ReplaceDbContext);
    }

    private void ReplaceDbContext(IServiceCollection services)
    {
        var existingDbContextRegistration = services.SingleOrDefault(
            d => d.ServiceType == typeof(DbContextOptions<EventStoreDbContext>)
        );

        if (existingDbContextRegistration != null)
        {
            services.Remove(existingDbContextRegistration);
        }

        services.AddDbContext<EventStoreDbContext>(options =>
            options.UseNpgsql(connectionString));

        // mock repositories for write side integration tests
        services.AddScoped(_ => mockTennisClubReadModelRepository.Object);
        services.AddScoped(_ => mockSubscriptionTierReadModelRepository.Object);
        services.AddScoped(_ => mockAdminReadModelRepository.Object);
        services.AddScoped(_ => mockMemberReadModelRepository.Object);
    }
}