using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RescueSystem.Domain.Entities;

namespace RescueSystem.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<Location> Locations { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<RescueRequest> Requests { get; set; }
        public DbSet<RescueTeam> RescueTeams { get; set; }
        public DbSet<Mission> Missions { get; set; }
        public DbSet<MissionHistory> MissionHistories { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<OtpCode> OtpCodes { get; set; }
        public DbSet<Checklist> Checklists { get; set; }
        public DbSet<ChecklistItem> ChecklistItems { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<RescueTeamJoinRequest> RescueTeamJoinRequests { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
            builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");

            // Convert UTC DateTime values to GMT+7 (+7 hours) when writing to database
            var dateTimeConverter = new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>(
                v => v.Kind == DateTimeKind.Utc ? v.AddHours(7) : v,
                v => v
            );

            var nullableDateTimeConverter = new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime?, DateTime?>(
                v => v.HasValue && v.Value.Kind == DateTimeKind.Utc ? v.Value.AddHours(7) : v,
                v => v
            );

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime))
                    {
                        property.SetValueConverter(dateTimeConverter);
                    }
                    else if (property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(nullableDateTimeConverter);
                    }
                }
            }
        }
    }
}
