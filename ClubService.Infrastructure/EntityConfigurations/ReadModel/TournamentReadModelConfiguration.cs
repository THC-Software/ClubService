using ClubService.Domain.ReadModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClubService.Infrastructure.EntityConfigurations.ReadModel;

public class TournamentReadModelConfiguration : IEntityTypeConfiguration<TournamentReadModel>
{
    public void Configure(EntityTypeBuilder<TournamentReadModel> builder)
    {
        builder.ToTable("Tournament");

        builder.HasKey(t => t.TournamentId);
        builder.Property(t => t.TournamentId)
            .IsRequired();

        builder.Property(t => t.TennisClubId)
            .IsRequired();

        builder.Property(t => t.Name)
            .IsRequired();

        builder.Property(t => t.StartDate)
            .IsRequired();

        builder.Property(t => t.EndDate)
            .IsRequired();
    }
}