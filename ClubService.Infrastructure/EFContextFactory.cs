using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ClubService.Infrastructure;

public class EFContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();

        var connectionString = configuration.GetConnectionString("MariaDbConnectionString");
        if (connectionString == null) 
        {
            throw new ArgumentNullException(null, "MariaDbConnectionString not found");
        }
        
        // builder.UseMySql(
        //     connectionString,
        //     ServerVersion.AutoDetect(connectionString)
        // );

        return new ApplicationDbContext(builder.Options);
    }
}