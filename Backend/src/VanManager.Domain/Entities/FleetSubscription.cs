namespace VanManager.Domain.Entities;

public class FleetSubscription
{
    public Guid Id { get; set; }
    public Guid FleetId { get; set; }
    public Guid PlanId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime? CancelledAt { get; set; }
    
    // Navigation properties
    public Fleet Fleet { get; set; } = null!;
    public Plan Plan { get; set; } = null!;
    public string TransactionId { get; set; }
    public string PaymentMethod { get; set; }
    public string Notes { get; set; }
}