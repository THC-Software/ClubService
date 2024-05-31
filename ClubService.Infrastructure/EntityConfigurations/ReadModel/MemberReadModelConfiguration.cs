using ClubService.Domain.Model.Enum;
using ClubService.Domain.Model.ValueObject;
using ClubService.Domain.ReadModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClubService.Infrastructure.EntityConfigurations.ReadModel;

public class MemberReadModelConfiguration : IEntityTypeConfiguration<MemberReadModel>
{
    public void Configure(EntityTypeBuilder<MemberReadModel> builder)
    {
        builder.ToTable("Member");
        
        builder.HasKey(m => m.MemberId);
        builder.Property(m => m.MemberId)
            .IsRequired()
            .HasConversion(
                memberId => memberId.Id,
                id => new MemberId(id)
            );
        
        builder.OwnsOne(m => m.Name, name =>
        {
            name.Property(fn => fn.FirstName)
                .HasColumnName("FirstName")
                .HasMaxLength(100)
                .IsRequired();
            name.Property(fn => fn.LastName)
                .HasColumnName("LastName")
                .HasMaxLength(100)
                .IsRequired();
        });
        
        builder.Property(m => m.Email)
            .IsRequired();
        
        builder.Property(m => m.TennisClubId)
            .IsRequired()
            .HasConversion(
                TennisClubId => TennisClubId.Id,
                id => new TennisClubId(id)
            );
        
        builder.Property(tc => tc.Status)
            .IsRequired()
            .HasConversion(
                s => s.ToString(),
                s => (MemberStatus)Enum.Parse(typeof(MemberStatus), s)
            );
    }
}