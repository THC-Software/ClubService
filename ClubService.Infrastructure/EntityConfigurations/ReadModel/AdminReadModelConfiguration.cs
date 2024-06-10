using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.ReadModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClubService.Infrastructure.EntityConfigurations.ReadModel;

public class AdminReadModelConfiguration : IEntityTypeConfiguration<AdminReadModel>
{
    public void Configure(EntityTypeBuilder<AdminReadModel> builder)
    {
        builder.ToTable("Admin");

        builder.HasKey(x => x.AdminId);

        builder.Property(x => x.AdminId)
            .HasConversion(
                v => v.Id,
                v => new AdminId(v))
            .IsRequired();

        builder.Property(x => x.Username)
            .HasMaxLength(100)
            .IsRequired();

        builder.OwnsOne(x => x.Name, name =>
        {
            name.Property(fn => fn.FirstName).HasColumnName("FirstName").HasMaxLength(100).IsRequired();
            name.Property(fn => fn.LastName).HasColumnName("LastName").HasMaxLength(100).IsRequired();
        });

        builder.Property(x => x.TennisClubId)
            .HasConversion(
                v => v.Id,
                v => new TennisClubId(v))
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .IsRequired();
    }
}