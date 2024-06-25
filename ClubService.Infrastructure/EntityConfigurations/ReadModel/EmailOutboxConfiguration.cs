using ClubService.Domain.ReadModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClubService.Infrastructure.EntityConfigurations.ReadModel;

public class EmailOutboxConfiguration : IEntityTypeConfiguration<EmailMessage>
{
    public void Configure(EntityTypeBuilder<EmailMessage> builder)
    {
        builder.ToTable("EmailOutbox");

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