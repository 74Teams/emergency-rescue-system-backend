using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RescueSystem.Domain.Entities;
using RescueSystem.Domain.Enums;

namespace RescueSystem.Infrastructure.Persistence.Configurations
{
    public class RescueTeamConfiguration : IEntityTypeConfiguration<RescueTeam>
    {
        public void Configure(EntityTypeBuilder<RescueTeam> builder)
        {
            builder.ToTable("RescueTeams");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(e => e.TeamName)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(e => e.TeamLeaderId)
                .IsRequired();

            builder.Property(e => e.BaseLocationId)
                .IsRequired();

            builder.Property(e => e.Status)
                .IsRequired()
                .HasDefaultValue(TeamStatus.AVAILABLE);

            builder.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasOne(e => e.TeamLeader)
                .WithMany(u => u.LeadingTeams)
                .HasForeignKey(e => e.TeamLeaderId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(e => e.BaseLocation)
                .WithMany()
                .HasForeignKey(e => e.BaseLocationId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(e => e.Members)
                .WithOne(u => u.MemberOfTeam)
                .HasForeignKey(u => u.RescueTeamId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
