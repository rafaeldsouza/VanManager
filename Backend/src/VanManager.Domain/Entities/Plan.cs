namespace VanManager.Domain.Entities;

public class Plan
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int MaxVans { get; set; }
    public bool Active { get; set; }
    public bool Visible { get; set; }
    
    // Navigation properties
    public ICollection<FleetSubscription> Subscriptions { get; set; } = new List<FleetSubscription>();
}