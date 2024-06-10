using ClubService.Domain.Model.Entity;
using ClubService.Domain.Model.ValueObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClubService.Infrastructure.EntityConfigurations.Login;

public class UserPasswordConfiguration : IEntityTypeConfiguration<UserPassword>
{
    public void Configure(EntityTypeBuilder<UserPassword> builder)
    {
        builder.ToTable("UserPassword");

        builder.HasKey(x => x.UserId);
        builder.Property(x => x.UserId)
            .HasConversion(
                v => v.Id,
                v => new UserId(v)
            )
            .IsRequired();
        builder.Property(x => x.HashedPassword)
            .IsRequired();
    }
}