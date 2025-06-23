using System.Linq.Expressions;
using VanManager.Domain.Entities;

namespace VanManager.Domain.Specifications;

public static class FleetSpecifications
{
    public static Expression<Func<Fleet, bool>> IsActive() =>
        fleet => fleet.IsActive;

    public static Expression<Func<Fleet, bool>> HasActiveVans() =>
        fleet => fleet.Vans.Any(v => v.IsActive);

    public static Expression<Func<Fleet, bool>> HasDriver(Guid driverId) =>
        fleet => fleet.Vans.Any(v => v.DriverId == driverId);

    public static Expression<Func<Fleet, bool>> HasActiveDriver(Guid driverId) =>
        fleet => fleet.Vans.Any(v => v.IsActive && v.DriverId == driverId);

    public static Expression<Func<Fleet, bool>> HasRoute(Guid routeId) =>
        fleet => fleet.Vans.Any(v => v.Routes.Any(r => r.Id == routeId));

    public static Expression<Func<Fleet, bool>> HasActiveRoute(Guid routeId) =>
        fleet => fleet.Vans.Any(v => v.IsActive && v.Routes.Any(r => r.IsActive && r.Id == routeId));

    public static Expression<Func<Fleet, bool>> OwnedBy(Guid ownerId) => 
        fleet => fleet.OwnerUserId == ownerId;
    
    public static Expression<Func<Fleet, bool>> HasActiveSubscription() => 
        fleet => fleet.Subscriptions.Any(s => s.IsActive && s.EndDate > DateTime.UtcNow);
    
    public static Expression<Func<Fleet, bool>> WithinPlanLimits() => 
        fleet => fleet.Subscriptions
            .Any(s => s.IsActive && 
                     s.EndDate > DateTime.UtcNow && 
                     fleet.Vans.Count <= s.Plan.MaxVans);
    
    public static Expression<Func<Fleet, bool>> CanAddVan() => 
        fleet => fleet.Subscriptions
            .Any(s => s.IsActive && 
                     s.EndDate > DateTime.UtcNow && 
                     fleet.Vans.Count < s.Plan.MaxVans);
    
    public static Expression<Func<Fleet, bool>> CanAddDriver() => 
        fleet => fleet.Subscriptions
            .Any(s => s.IsActive && 
                     s.EndDate > DateTime.UtcNow);
    
    public static Expression<Func<Fleet, bool>> CanAddRoute() => 
        fleet => fleet.Subscriptions
            .Any(s => s.IsActive && 
                     s.EndDate > DateTime.UtcNow);
} 