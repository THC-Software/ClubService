using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.ReadModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClubService.Infrastructure.EntityConfigurations.ReadModel;

public class SubscriptionTierReadModelConfiguration : IEntityTypeConfiguration<SubscriptionTierReadModel>
{
    public void Configure(EntityTypeBuilder<SubscriptionTierReadModel> builder)
    {
        builder.ToTable("SubscriptionTier");
        
        builder.HasKey(s => s.SubscriptionTierId);
        builder.Property(s => s.SubscriptionTierId)
            .IsRequired()
            .HasConversion(
                v => v.Id,
                v => new SubscriptionTierId(v));
        builder.Property(s => s.Name)
            .IsRequired();
        builder.Property(s => s.MaxMemberCount)
            .IsRequired();
    }
}