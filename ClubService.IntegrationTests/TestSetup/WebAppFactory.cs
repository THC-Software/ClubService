using ClubService.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ClubService.IntegrationTests.TestSetup;

public class WebAppFactory(string connectionString) : WebApplicationFactory<Program>
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
    }
}