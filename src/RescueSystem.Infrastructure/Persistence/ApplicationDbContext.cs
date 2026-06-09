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

            // Convert DateTime values to GMT+7 and force Kind=Unspecified for Npgsql compatibility
            // Npgsql 8+ strictly requires Kind=Unspecified for 'timestamp without time zone' columns
            var dateTimeConverter = new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>(
                v => DateTime.SpecifyKind(v.Kind == DateTimeKind.Utc ? v.AddHours(7) : v, DateTimeKind.Unspecified),
                v => DateTime.SpecifyKind(v, DateTimeKind.Unspecified)
            );

            var nullableDateTimeConverter = new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime?, DateTime?>(
                v => v.HasValue
                    ? DateTime.SpecifyKind(v.Value.Kind == DateTimeKind.Utc ? v.Value.AddHours(7) : v.Value, DateTimeKind.Unspecified)
                    : v,
                v => v.HasValue
                    ? DateTime.SpecifyKind(v.Value, DateTimeKind.Unspecified)
                    : v
            );

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime))
                    {
                        property.SetValueConverter(dateTimeConverter);
                        property.SetColumnType("timestamp without time zone");
                    }
                    else if (property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(nullableDateTimeConverter);
                        property.SetColumnType("timestamp without time zone");
                    }
                }
            }
        }
    }
}
