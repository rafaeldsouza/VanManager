namespace VanManager.Domain.Entities;

public class StudentTripLog
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public Guid RouteId { get; set; }
    public Guid VanId { get; set; }
    public DateTime BoardingTime { get; set; }
    
    // Coordenadas de embarque
    public double BoardingLatitude { get; set; }
    public double BoardingLongitude { get; set; }
    public string? BoardingAddress { get; set; }
    
    // Coordenadas de desembarque
    public double DropoffLatitude { get; set; }
    public double DropoffLongitude { get; set; }
    public string? DropoffAddress { get; set; }
    
    public DateTime DropoffTime { get; set; }
    public string? Notes { get; set; }
    public TripStatus Status { get; set; }
    
    // Navigation properties
    public Student Student { get; set; } = null!;
    public Route Route { get; set; } = null!;
    public Van Van { get; set; } = null!;
}

public enum TripStatus
{
    Scheduled,
    InProgress,
    Completed,
    Cancelled,
    NoShow
}