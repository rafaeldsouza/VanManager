using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Constants;
using VanManager.Domain.Entities;
using VanManager.Domain.Repositories;
using VanManager.Infrastructure.Data;
using VanManager.Infrastructure.Identity;
using VanManager.Infrastructure.Repositories;
using VanManager.Infrastructure.Services;

namespace VanManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<ApplicationDbContextInitializer>();
        
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services
            .AddIdentity<AppUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                
                options.User.RequireUniqueEmail = true;
                
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddTransient<IIdentityService, IdentityService>();
        services.AddTransient<ITokenService, TokenService>();
        services.AddTransient<IEmailService, EmailService>();
        services.AddTransient<IDateTimeProvider, DateTimeProvider>();

        // Configure JWT Authentication
        var jwtSettings = configuration.GetSection("JwtSettings");
        var key = Encoding.ASCII.GetBytes(jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret not configured"));
        
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = true;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                ClockSkew = TimeSpan.Zero
            };
        });

        // Configure authorization policies
        services.AddAuthorization(options =>
        {
            // Role-based policies
            options.AddPolicy(Policies.IsAdmin, policy => policy.RequireRole(Roles.Admin));
            options.AddPolicy(Policies.IsFleetOwner, policy => policy.RequireRole(Roles.FleetOwner));
            options.AddPolicy(Policies.IsDriver, policy => policy.RequireRole(Roles.Driver));
            options.AddPolicy(Policies.IsParent, policy => policy.RequireRole(Roles.Parent));
            
            // Permission-based policies
            options.AddPolicy(Permissions.ViewUsers, policy => policy.RequireRole(Roles.Admin));
            options.AddPolicy(Permissions.CreateUsers, policy => policy.RequireRole(Roles.Admin));
            options.AddPolicy(Permissions.EditUsers, policy => policy.RequireRole(Roles.Admin));
            options.AddPolicy(Permissions.DeleteUsers, policy => policy.RequireRole(Roles.Admin));
            
            options.AddPolicy(Permissions.ViewFleets, policy => 
                policy.RequireAssertion(context => 
                    context.User.IsInRole(Roles.Admin) || 
                    context.User.IsInRole(Roles.FleetOwner)));
            
            options.AddPolicy(Permissions.CreateFleets, policy => 
                policy.RequireAssertion(context => 
                    context.User.IsInRole(Roles.Admin) || 
                    context.User.IsInRole(Roles.FleetOwner)));
            
            options.AddPolicy(Permissions.EditFleets, policy => 
                policy.RequireAssertion(context => 
                    context.User.IsInRole(Roles.Admin) || 
                    context.User.IsInRole(Roles.FleetOwner)));
            
            options.AddPolicy(Permissions.DeleteFleets, policy => 
                policy.RequireAssertion(context => 
                    context.User.IsInRole(Roles.Admin) || 
                    context.User.IsInRole(Roles.FleetOwner)));
            
            // Add more permission-based policies as needed
        });

        // Serilog configuration
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        return services;
    }
}