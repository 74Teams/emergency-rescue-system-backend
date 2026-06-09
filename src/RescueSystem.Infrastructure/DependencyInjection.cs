using Autofac.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RescueSystem.Application.Common.ExternalSettings;
using RescueSystem.Infrastructure.Common.Interfaces.Repositories;
using RescueSystem.Application.Common.Interfaces.Services;
using RescueSystem.Application.Common.Interfaces.Repositories;
using RescueSystem.Domain.Entities;
using RescueSystem.Infrastructure.Persistence;
using RescueSystem.Infrastructure.Persistence.Repositories;
using RescueSystem.Infrastructure.Persistence.Services;

namespace RescueSystem.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add DbContext
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(
                    connectionString,
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
            });
            // Add Identity services
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // Add repositories 
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IRequestRespository, RequestRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<ILocationRepository, LocationRepository>();
            services.AddScoped<IMissionRepository, MissionRepository>();
            services.AddScoped<IContactRepository, ContactRepository>();
            services.AddScoped<IChecklistRepository, ChecklistRepository>();
            services.AddScoped<IChecklistItemRepository, ChecklistItemRepository>();

            // Add Email service
            services.AddScoped<IEmailService, EmailService>();

            // Add Otp service
            services.AddScoped<IOtpService, OtpService>();
            // Add Cloudinary service
            services.Configure<CloudinarySetting>(
                configuration.GetSection("Cloudinary"));
            services.AddScoped<ICloudinaryService, CloudinaryService>();
            services.AddScoped<IRescueTeamRepository, RescueTeamRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<IRescueTeamJoinRequestRepository, RescueTeamJoinRequestRepository>();
            services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();
            return services;
        }
    }
}
