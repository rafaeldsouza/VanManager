namespace VanManager.Domain.Entities;

public class RouteStop
{
    public Guid Id { get; set; }
    public Guid RouteId { get; set; }
    public Guid StudentId { get; set; }
    public DateTime Timestamp { get; set; }
    public string Type { get; set; } = string.Empty; // EMBARQUE, DESEMBARQUE
    public decimal LocationLat { get; set; }
    public decimal LocationLng { get; set; }
    
    // Navigation properties
    public Route Route { get; set; } = null!;
    public Student Student { get; set; } = null!;
}