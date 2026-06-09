using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RescueSystem.Domain.Entities;

namespace RescueSystem.Infrastructure.Persistence.Configurations
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable("Addresses");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                    .HasDefaultValueSql("gen_random_uuid()");

            builder.Property(e => e.UserId)
                .IsRequired();

            builder.Property(e => e.Street)
                .HasMaxLength(255);

            builder.Property(e => e.City)
                .HasMaxLength(100);

            builder.Property(e => e.District)
                .HasMaxLength(100);

            builder.Property(e => e.GPS)
                .HasMaxLength(255);

            builder.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasOne(e => e.User)
                .WithOne()
                .HasForeignKey<Address>(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

