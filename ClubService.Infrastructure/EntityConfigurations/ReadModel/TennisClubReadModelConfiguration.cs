using ClubService.Domain.Model.Enum;
using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.ReadModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClubService.Infrastructure.EntityConfigurations.ReadModel;

public class TennisClubReadModelConfiguration : IEntityTypeConfiguration<TennisClubReadModel>
{
    public void Configure(EntityTypeBuilder<TennisClubReadModel> builder)
    {
        builder.ToTable("TennisClub");
        
        builder.HasKey(tc => tc.TennisClubId);
        builder.Property(tc => tc.TennisClubId)
            .IsRequired()
            .HasConversion(
                tci => tci.Id,
                tci => new TennisClubId(tci));
        builder.Property(tc => tc.Name)
            .IsRequired();
        builder.Property(tc => tc.SubscriptionTierId)
            .IsRequired()
            .HasConversion(
                sti => sti.Id,
                sti => new SubscriptionTierId(sti));
        builder.Property(tc => tc.Status)
            .IsRequired()
            .HasConversion(
                s => s.ToString(),
                s => (TennisClubStatus)Enum.Parse(typeof(TennisClubStatus), s)
            );
        builder.Property(tc => tc.MemberCount)
            .IsRequired();
    }
}