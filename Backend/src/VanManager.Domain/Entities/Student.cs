namespace VanManager.Domain.Entities;

public class Student
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }
    public string? Document { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public Guid? VanId { get; set; }
    public Guid FleetId { get; set; }
    public Guid SchoolId { get; set; }
    public Guid ParentId { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }

    // Navigation properties
    public AppUser Parent { get; set; } = null!;
    public Van? Van { get; set; }
    public Fleet Fleet { get; set; } = null!;
    public ICollection<RouteStop> RouteStops { get; set; } = new List<RouteStop>();
    public ICollection<StudentAbsence> Absences { get; set; } = new List<StudentAbsence>();
    public ICollection<TripCheckout> Checkouts { get; set; } = new List<TripCheckout>();
    public ICollection<AppUser> Guardians { get; set; } = new List<AppUser>();
    public ICollection<StudentTripLog> TripLogs { get; set; } = new List<StudentTripLog>();
    public ICollection<ParentInvitation> ParentInvitations { get; set; } = new List<ParentInvitation>();
    public string Email { get; set; }
    public string ProfilePictureUrl { get; set; }
    public string Notes { get; set; }
}