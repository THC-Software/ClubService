using ClubService.Domain.Event;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace ClubService.Infrastructure.EntityConfigurations;

public class DomainEnvelopeConfiguration : IEntityTypeConfiguration<DomainEnvelope<IDomainEvent>>
{
    public void Configure(EntityTypeBuilder<DomainEnvelope<IDomainEvent>> builder)
    {
        // In PostgresEventRepository we don't use the ORM for saving and loading events
        // Nevertheless the mapping here will remain because then it creates the table for dev and test
        // It also enables seeding data in EventStoreDbContext
        builder.ToTable("DomainEvent");
        
        builder.HasKey(e => e.EventId);
        
        builder.Property(e => e.EventId)
            .IsRequired();
        
        builder.Property(e => e.EntityId)
            .IsRequired();
        
        builder.Property(e => e.EventType)
            .HasConversion(
                e => e.ToString(),
                e => (EventType)Enum.Parse(typeof(EventType), e)
            )
            .IsRequired();
        
        builder.Property(e => e.EntityType)
            .HasConversion(
                e => e.ToString(),
                e => (EntityType)Enum.Parse(typeof(EntityType), e)
            )
            .IsRequired();
        
        builder.Property(e => e.Timestamp)
            .IsRequired();
        
        builder.Property(e => e.EventData)
            .HasConversion<string>(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<IDomainEvent>(v)!);
        
        builder.Property(e => e.CorrelationId)
            .IsRequired(false);
    }
}