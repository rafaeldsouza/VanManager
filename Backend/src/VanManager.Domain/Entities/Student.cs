namespace VanManager.Domain.Entities;

public class Student
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public Guid SchoolId { get; set; }
    public Guid ParentId { get; set; }
    
    // Navigation properties
    public AppUser Parent { get; set; } = null!;
    public ICollection<RouteStop> RouteStops { get; set; } = new List<RouteStop>();
    public ICollection<StudentAbsence> Absences { get; set; } = new List<StudentAbsence>();
    public ICollection<TripCheckout> Checkouts { get; set; } = new List<TripCheckout>();
    public ICollection<AuthorizedGuardian> AuthorizedGuardians { get; set; } = new List<AuthorizedGuardian>();
    public ICollection<StudentTripLog> TripLogs { get; set; } = new List<StudentTripLog>();
}