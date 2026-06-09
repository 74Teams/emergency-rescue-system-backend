using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RescueSystem.Domain.Entities;

namespace RescueSystem.Infrastructure.Persistence.Configurations
{
    public class LeaveRequestConfiguration : IEntityTypeConfiguration<LeaveRequest>
    {
        public void Configure(EntityTypeBuilder<LeaveRequest> builder)
        {
            builder.ToTable("LeaveRequests");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Reason)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(x => x.Note)
                .HasMaxLength(1000);

            builder.HasOne(x => x.Rescuer)
                .WithMany()
                .HasForeignKey(x => x.RescuerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.RescueTeam)
                .WithMany()
                .HasForeignKey(x => x.RescueTeamId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
