using ClubService.Domain.Event;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace ClubService.Infrastructure.EntityConfigurations;

public class DomainEnvelopeConfiguration : IEntityTypeConfiguration<DomainEnvelope<IDomainEvent>>
{
    public void Configure(EntityTypeBuilder<DomainEnvelope<IDomainEvent>> builder)
    {
        builder.ToTable("DomainEvent");
        
        builder.HasKey(e => e.Id);
        
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

        builder.Property(e => e.DomainEvent)
            .HasConversion<string>(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<IDomainEvent>(v));
    }
}