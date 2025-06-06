namespace VanManager.Domain.Entities;

public class StudentTripLog
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public Guid RouteId { get; set; }
    public DateTime BoardingTime { get; set; }
    public Guid BoardingLocationId { get; set; }
    public DateTime DropoffTime { get; set; }
    public Guid DropoffLocationId { get; set; }
    
    // Navigation properties
    public Student Student { get; set; } = null!;
    public Route Route { get; set; } = null!;
}