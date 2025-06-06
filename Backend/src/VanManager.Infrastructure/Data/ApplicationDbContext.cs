using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VanManager.Application.Common.Interfaces;
using VanManager.Domain.Entities;

namespace VanManager.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>, IApplicationDbContext

{
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider)
        : base(options)
    {
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
    }

    public DbSet<Fleet> Fleets => Set<Fleet>();
    public DbSet<Van> Vans => Set<Van>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<School> Schools => Set<School>();
    public DbSet<Route> Routes => Set<Route>();
    public DbSet<RouteStop> RouteStops => Set<RouteStop>();
    public DbSet<StudentAbsence> StudentAbsences => Set<StudentAbsence>();
    public DbSet<TripCheckout> TripCheckouts => Set<TripCheckout>();
    public DbSet<AuthorizedGuardian> AuthorizedGuardians => Set<AuthorizedGuardian>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Plan> Plans => Set<Plan>();
    public DbSet<FleetSubscription> FleetSubscriptions => Set<FleetSubscription>();
    public DbSet<StudentTripLog> StudentTripLogs => Set<StudentTripLog>();
    public DbSet<Referral> Referrals => Set<Referral>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure entity relationships and constraints
        ConfigureFleetEntity(builder);
        ConfigureVanEntity(builder);
        ConfigureStudentEntity(builder);
        ConfigureRouteEntity(builder);
        ConfigureRouteStopEntity(builder);
        ConfigureStudentAbsenceEntity(builder);
        ConfigureTripCheckoutEntity(builder);
        ConfigureAuthorizedGuardianEntity(builder);
        ConfigureNotificationEntity(builder);
        ConfigurePlanEntity(builder);
        ConfigureFleetSubscriptionEntity(builder);
        ConfigureStudentTripLogEntity(builder);
        ConfigureReferralEntity(builder);
        ConfigureRefreshTokenEntity(builder);
        ConfigureAppUserEntity(builder);
    }

    private void ConfigureFleetEntity(ModelBuilder builder)
    {
        builder.Entity<Fleet>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasOne(e => e.Owner)
                .WithMany(e => e.OwnedFleets)
                .HasForeignKey(e => e.OwnerUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureVanEntity(ModelBuilder builder)
    {
        builder.Entity<Van>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.PlateNumber)
                .IsRequired()
                .HasMaxLength(20);

            entity.HasOne(e => e.Fleet)
                .WithMany(e => e.Vans)
                .HasForeignKey(e => e.FleetId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.AssignedDriver)
                .WithMany()
                .HasForeignKey(e => e.AssignedDriverId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureStudentEntity(ModelBuilder builder)
    {
        builder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.FullName)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasOne(e => e.Parent)
                .WithMany(e => e.Children)
                .HasForeignKey(e => e.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureRouteEntity(ModelBuilder builder)
    {
        builder.Entity<Route>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(200);

            entity.HasOne(e => e.Van)
                .WithMany(e => e.Routes)
                .HasForeignKey(e => e.VanId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureRouteStopEntity(ModelBuilder builder)
    {
        builder.Entity<RouteStop>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Type)
                .IsRequired()
                .HasMaxLength(20);

            entity.HasOne(e => e.Route)
                .WithMany(e => e.Stops)
                .HasForeignKey(e => e.RouteId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Student)
                .WithMany(e => e.RouteStops)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigureStudentAbsenceEntity(ModelBuilder builder)
    {
        builder.Entity<StudentAbsence>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Type)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(e => e.Reason)
                .HasMaxLength(500);

            entity.HasOne(e => e.Student)
                .WithMany(e => e.Absences)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Route)
                .WithMany(e => e.Absences)
                .HasForeignKey(e => e.RouteId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigureTripCheckoutEntity(ModelBuilder builder)
    {
        builder.Entity<TripCheckout>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Student)
                .WithMany(e => e.Checkouts)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Responsible)
                .WithMany(e => e.ResponsibleCheckouts)
                .HasForeignKey(e => e.ResponsibleId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Route)
                .WithMany(e => e.Checkouts)
                .HasForeignKey(e => e.RouteId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigureAuthorizedGuardianEntity(ModelBuilder builder)
    {
        builder.Entity<AuthorizedGuardian>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.FullName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20);

            entity.Property(e => e.DocumentId)
                .HasMaxLength(50);

            entity.Property(e => e.Relationship)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Description)
                .HasMaxLength(300);

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.HasOne(e => e.Student)
                .WithMany(e => e.AuthorizedGuardians)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

    }

    private void ConfigureNotificationEntity(ModelBuilder builder)
    {
        builder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.NotificationType)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(e => e.Message)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(20);

            entity.HasOne(e => e.User)
                .WithMany(e => e.Notifications)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigurePlanEntity(ModelBuilder builder)
    {
        builder.Entity<Plan>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Price)
                .HasPrecision(10, 2);
        });
    }

    private void ConfigureFleetSubscriptionEntity(ModelBuilder builder)
    {
        builder.Entity<FleetSubscription>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Fleet)
                .WithMany()
                .HasForeignKey(e => e.FleetId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Plan)
                .WithMany(e => e.Subscriptions)
                .HasForeignKey(e => e.PlanId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureStudentTripLogEntity(ModelBuilder builder)
    {
        builder.Entity<StudentTripLog>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Student)
                .WithMany(e => e.TripLogs)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Route)
                .WithMany(e => e.TripLogs)
                .HasForeignKey(e => e.RouteId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigureReferralEntity(ModelBuilder builder)
    {
        builder.Entity<Referral>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(20);

            entity.HasOne(e => e.Referrer)
                .WithMany()
                .HasForeignKey(e => e.ReferrerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Referred)
                .WithMany()
                .HasForeignKey(e => e.ReferredId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private void ConfigureRefreshTokenEntity(ModelBuilder builder)
    {
        builder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Token)
                .IsRequired()
                .HasMaxLength(200);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigureAppUserEntity(ModelBuilder builder)
    {
        builder.Entity<AppUser>(entity =>
        {
            entity.Property(e => e.FullName)
                .HasMaxLength(100);
            entity.Property(e => e.Document)
                .HasMaxLength(30);
        });
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}