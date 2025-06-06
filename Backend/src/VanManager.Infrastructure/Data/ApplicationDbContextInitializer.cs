using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VanManager.Domain.Constants;
using VanManager.Domain.Entities;

namespace VanManager.Infrastructure.Data;

public class ApplicationDbContextInitializer
{
    private readonly ILogger<ApplicationDbContextInitializer> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public ApplicationDbContextInitializer(
        ILogger<ApplicationDbContextInitializer> logger,
        ApplicationDbContext context,
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitializeAsync()
    {
        try
        {
            if (_context.Database.IsNpgsql())
            {
                await _context.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initializing the database");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    private async Task TrySeedAsync()
    {
        // Default roles
        var roles = Roles.All;
        
        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
                _logger.LogInformation("Created role {Role}", role);
            }
        }

        // Default admin user
        var administratorEmail = "admin@vanmanager.com";
        var administratorPassword = "Admin@123456";

        if (await _userManager.FindByEmailAsync(administratorEmail) == null)
        {
            var administrator = new AppUser
            {
                UserName = administratorEmail,
                Email = administratorEmail,
                EmailConfirmed = true,
                FullName = "System Administrator",
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(administrator, administratorPassword);
            
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(administrator, Roles.Admin);
                _logger.LogInformation("Created admin user {Email}", administratorEmail);
            }
            else
            {
                _logger.LogError("Failed to create admin user. Errors: {Errors}", 
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        // Seed sample data for demo purposes if needed
        await SeedSampleDataAsync();
    }

    private async Task SeedSampleDataAsync()
    {
        // Sample plans
        if (!await _context.Plans.AnyAsync())
        {
            var plans = new List<Plan>
            {
                new() { 
                    Id = Guid.NewGuid(), 
                    Name = "Básico", 
                    Price = 99.90m, 
                    MaxVans = 1, 
                    Active = true, 
                    Visible = true 
                },
                new() { 
                    Id = Guid.NewGuid(), 
                    Name = "Profissional", 
                    Price = 299.90m, 
                    MaxVans = 5, 
                    Active = true, 
                    Visible = true 
                },
                new() { 
                    Id = Guid.NewGuid(), 
                    Name = "Empresarial", 
                    Price = 999.90m, 
                    MaxVans = 20, 
                    Active = true, 
                    Visible = true 
                }
            };
            
            await _context.Plans.AddRangeAsync(plans);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Seeded sample plans");
        }
    }
}