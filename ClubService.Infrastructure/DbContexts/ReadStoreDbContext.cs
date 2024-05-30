using Microsoft.EntityFrameworkCore;

namespace ClubService.Infrastructure.DbContexts;

public class ReadStoreDbContext(DbContextOptions<ReadStoreDbContext> options) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseCamelCaseNamingConvention();
    }
}