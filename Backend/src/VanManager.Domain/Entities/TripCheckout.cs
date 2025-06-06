namespace VanManager.Domain.Entities;

public class TripCheckout
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public Guid ResponsibleId { get; set; }
    public Guid RouteId { get; set; }
    public DateTime Timestamp { get; set; }
    
    // Navigation properties
    public Student Student { get; set; } = null!;
    public AppUser Responsible { get; set; } = null!;
    public Route Route { get; set; } = null!;
}