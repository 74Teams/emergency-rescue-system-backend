using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RescueSystem.Domain.Entities;
using RescueSystem.Domain.Enums;

namespace RescueSystem.Infrastructure.Persistence.Configurations
{
    public class RequestConfiguration : IEntityTypeConfiguration<RescueRequest>
    {
        public void Configure(EntityTypeBuilder<RescueRequest> builder)
        {
            builder.ToTable("Requests");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(e => e.UserId)
                .IsRequired(false);

            builder.Property(e => e.EmergencyType)
                .IsRequired();

            builder.Property(e => e.Priority)
                .IsRequired();

            builder.Property(e => e.Status)
                .IsRequired()
                .HasDefaultValue(RequestStatus.PENDING);

            builder.Property(e => e.LocationId)
                .IsRequired();

            builder.Property(e => e.Description)
                .HasMaxLength(2000);

            builder.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasOne(e => e.RequestedBy)
                .WithMany(u => u.Requests)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(e => e.Location)
                .WithMany()
                .HasForeignKey(e => e.LocationId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
