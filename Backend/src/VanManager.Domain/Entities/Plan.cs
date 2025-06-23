namespace VanManager.Domain.Entities;

public class Plan
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = "BRL";
    public string BillingCycle { get; set; } = "MONTHLY"; // MONTHLY, QUARTERLY, YEARLY
    public int DurationInMonths { get; set; } = 1; // Duração padrão em meses
    public bool IsDefault { get; set; } // Indica se é o plano padrão para novas frotas
    public int MaxVans { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public Guid? UpdatedByUserId { get; set; }

    // Navigation properties
    public AppUser? CreatedBy { get; set; }
    public AppUser? UpdatedBy { get; set; }
    public ICollection<FleetSubscription> FleetSubscriptions { get; set; } = new List<FleetSubscription>();
}