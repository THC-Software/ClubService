using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.ReadModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClubService.Infrastructure.EntityConfigurations.ReadModel;

public class SystemOperatorReadModelConfiguration : IEntityTypeConfiguration<SystemOperatorReadModel>
{
    public void Configure(EntityTypeBuilder<SystemOperatorReadModel> builder)
    {
        builder.ToTable("SystemOperator");

        builder.HasKey(so => so.SystemOperatorId);
        builder.Property(so => so.SystemOperatorId)
            .IsRequired()
            .HasConversion(
                soi => soi.Id,
                soi => new SystemOperatorId(soi));

        builder.Property(so => so.Username)
            .IsRequired();
    }
}