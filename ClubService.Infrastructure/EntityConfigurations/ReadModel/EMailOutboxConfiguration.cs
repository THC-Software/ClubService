using ClubService.Domain.ReadModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClubService.Infrastructure.EntityConfigurations.ReadModel;

public class EMailOutboxConfiguration : IEntityTypeConfiguration<EMailOutbox>
{
    public void Configure(EntityTypeBuilder<EMailOutbox> builder)
    {
        builder.ToTable("EMailOutbox");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .IsRequired();

        builder.Property(x => x.RecipientEMailAddress)
            .IsRequired();

        builder.Property(x => x.Subject)
            .IsRequired();

        builder.Property(x => x.Body)
            .IsRequired();

        builder.Property(x => x.Timestamp)
            .IsRequired();
    }
}