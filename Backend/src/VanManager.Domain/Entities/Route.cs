namespace VanManager.Domain.Entities;

public class Route
{
    public Guid Id { get; set; }
    public Guid VanId { get; set; }
    public string Description { get; set; } = string.Empty;
    
    // Navigation properties
    public Van Van { get; set; } = null!;
    public ICollection<RouteStop> Stops { get; set; } = new List<RouteStop>();
    public ICollection<StudentAbsence> Absences { get; set; } = new List<StudentAbsence>();
    public ICollection<TripCheckout> Checkouts { get; set; } = new List<TripCheckout>();
    public ICollection<StudentTripLog> TripLogs { get; set; } = new List<StudentTripLog>();
}