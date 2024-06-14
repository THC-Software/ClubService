using ClubService.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace ClubService.API.ApplicationConfigurations;

public static class DatabaseExtensions
{
    public static void AddDatabaseConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<EventStoreDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("event-store-connection"));
        });
        services.AddDbContext<ReadStoreDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("read-store-connection"));
        });
        services.AddDbContext<LoginStoreDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("login-store-connection"));
        });
    }
}