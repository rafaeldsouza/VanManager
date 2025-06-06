using Microsoft.EntityFrameworkCore;
using VanManager.Domain.Entities;

namespace VanManager.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Fleet> Fleets { get; }
    DbSet<Van> Vans { get; }
    DbSet<Student> Students { get; }
    DbSet<School> Schools { get; }
    DbSet<Route> Routes { get; }
    DbSet<RouteStop> RouteStops { get; }
    DbSet<StudentAbsence> StudentAbsences { get; }
    DbSet<TripCheckout> TripCheckouts { get; }
    DbSet<AuthorizedGuardian> AuthorizedGuardians { get; }
    DbSet<Notification> Notifications { get; }
    DbSet<Plan> Plans { get; }
    DbSet<FleetSubscription> FleetSubscriptions { get; }
    DbSet<StudentTripLog> StudentTripLogs { get; }
    DbSet<Referral> Referrals { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}