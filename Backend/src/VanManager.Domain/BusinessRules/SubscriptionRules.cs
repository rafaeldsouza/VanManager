using VanManager.Domain.Entities;
using VanManager.Domain.Specifications;

namespace VanManager.Domain.BusinessRules;

public static class SubscriptionRules
{
    public static bool CanCreateFleet(AppUser user)
    {
        // Verifica se o usuário já tem uma frota ativa
        var hasActiveFleet = user.OwnedFleets.Any(f => f.IsActive);
        
        // Se não tem frota ativa, pode criar
        if (!hasActiveFleet) return true;
        
        // Se tem frota, verifica se tem assinatura ativa
        var hasActiveSubscription = user.OwnedFleets
            .Any(f => f.Subscription != null);
            
        return hasActiveSubscription;
    }
    
    public static bool CanAddVan(Fleet fleet, Plan plan)
    {
        if (!fleet.IsActive || !plan.IsActive)
            return false;

        var activeSubscription = fleet.Subscriptions
            .FirstOrDefault(s => s.IsActive && s.EndDate > DateTime.UtcNow);

        if (activeSubscription == null)
            return false;

        return fleet.Vans.Count < plan.MaxVans;
    }
    
    public static bool CanAddDriver(Fleet fleet, Plan plan)
    {
        if (!fleet.IsActive || !plan.IsActive)
            return false;

        var activeSubscription = fleet.Subscriptions
            .FirstOrDefault(s => s.IsActive && s.EndDate > DateTime.UtcNow);

        if (activeSubscription == null)
            return false;

        return true;
    }
    
    public static bool CanAddRoute(Fleet fleet, Plan plan)
    {
        if (!fleet.IsActive || !plan.IsActive)
            return false;

        var activeSubscription = fleet.Subscriptions
            .FirstOrDefault(s => s.IsActive && s.EndDate > DateTime.UtcNow);

        if (activeSubscription == null)
            return false;

        return true;
    }
    
    public static bool IsSubscriptionExpiringSoon(FleetSubscription subscription)
    {
        var daysUntilExpiration = (subscription.EndDate - DateTime.UtcNow).TotalDays;
        return daysUntilExpiration <= 7 && daysUntilExpiration > 0;
    }
    
    public static bool CanRenewSubscription(FleetSubscription subscription)
    {
        // Só pode renovar se a assinatura estiver ativa ou expirada há menos de 30 dias
        if (subscription.IsActive) return true;
        
        var daysSinceExpiration = (DateTime.UtcNow - subscription.EndDate).TotalDays;
        return daysSinceExpiration <= 30;
    }
} 