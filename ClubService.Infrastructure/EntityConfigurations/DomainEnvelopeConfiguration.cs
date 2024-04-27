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
        
        builder.HasKey(e => e.EventId);

        builder.Property(e => e.EntityId)
            .IsRequired();

        builder.Property(e => e.EventType)
            .IsRequired();

        builder.Property(e => e.EntityType)
            .IsRequired();

        builder.Property(e => e.DomainEvent)
            .HasConversion<string>(
                v => JsonConvert.SerializeObject(v),
                v => JsonConvert.DeserializeObject<IDomainEvent>(v));

    }
}