using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RescueSystem.Domain.Entities;
using RescueSystem.Domain.Enums;

namespace RescueSystem.Infrastructure.Persistence.Configurations
{
    public class MissionConfiguration : IEntityTypeConfiguration<Mission>
    {
        public void Configure(EntityTypeBuilder<Mission> builder)
        {
            builder.ToTable("Missions");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(e => e.RequestId)
                .IsRequired();

            builder.Property(e => e.DispatcherId)
                .IsRequired();

            builder.Property(e => e.RescueTeamId)
                .IsRequired();

            builder.Property(e => e.StartTime)
                .IsRequired();

            builder.Property(e => e.EndTime);

            builder.Property(e => e.Status)
                .IsRequired()
                .HasDefaultValue(MissionStatus.ASSIGNED);

            builder.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasOne(e => e.Request)
                .WithMany(r => r.Missions)
                .HasForeignKey(e => e.RequestId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(e => e.Dispatcher)
                .WithMany(u => u.DispatchedMissions)
                .HasForeignKey(e => e.DispatcherId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(e => e.RescueTeam)
                .WithMany(rt => rt.Missions)
                .HasForeignKey(e => e.RescueTeamId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
