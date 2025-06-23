using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;
using VanManager.Infrastructure.Data.Configurations;

namespace VanManager.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Fleet> Fleets { get; set; } = null!;
    public DbSet<Van> Vans { get; set; } = null!;
    public DbSet<Student> Students { get; set; } = null!;
    public DbSet<School> Schools { get; set; } = null!;
    public DbSet<Route> Routes { get; set; } = null!;
    public DbSet<RouteStop> RouteStops { get; set; } = null!;
    public DbSet<StudentAbsence> StudentAbsences { get; set; } = null!;
    public DbSet<TripCheckout> TripCheckouts { get; set; } = null!;
    public DbSet<AuthorizedGuardian> AuthorizedGuardians { get; set; } = null!;
    public DbSet<Notification> Notifications { get; set; } = null!;
    public DbSet<Plan> Plans { get; set; } = null!;
    public DbSet<FleetSubscription> FleetSubscriptions { get; set; } = null!;
    public DbSet<StudentTripLog> StudentTripLogs { get; set; } = null!;
    public DbSet<Referral> Referrals { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<ParentInvitation> ParentInvitations { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Aplica as configurações
        builder.ApplyConfiguration(new RoleConfiguration());
        builder.ApplyConfiguration(new AppUserConfiguration());
        builder.ApplyConfiguration(new FleetConfiguration());
        builder.ApplyConfiguration(new VanConfiguration());
        builder.ApplyConfiguration(new RouteConfiguration());
        builder.ApplyConfiguration(new StudentConfiguration());
        builder.ApplyConfiguration(new StudentTripLogConfiguration());
        builder.ApplyConfiguration(new StudentAbsenceConfiguration());
        builder.ApplyConfiguration(new ParentInvitationConfiguration());
        builder.ApplyConfiguration(new FleetSubscriptionConfiguration());
        builder.ApplyConfiguration(new PlanConfiguration());
        builder.ApplyConfiguration(new ReferralConfiguration());
        builder.ApplyConfiguration(new RefreshTokenConfiguration());
        builder.ApplyConfiguration(new AuthorizedGuardianConfiguration());
        builder.ApplyConfiguration(new NotificationConfiguration());
        builder.ApplyConfiguration(new RouteStopConfiguration());
        builder.ApplyConfiguration(new SchoolConfiguration());
        builder.ApplyConfiguration(new TripCheckoutConfiguration());
    }
}