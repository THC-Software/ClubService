using ClubService.Domain.Event;
using ClubService.Infrastructure.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace ClubService.Infrastructure;

public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<DomainEnvelope<IDomainEvent>> DomainEvents { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseCamelCaseNamingConvention();
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new DomainEnvelopeConfiguration());
    }
}