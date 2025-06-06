namespace VanManager.Domain.Entities;

public class FleetSubscription
{
    public Guid Id { get; set; }
    public Guid FleetId { get; set; }
    public Guid PlanId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
    
    // Navigation properties
    public Fleet Fleet { get; set; } = null!;
    public Plan Plan { get; set; } = null!;
}