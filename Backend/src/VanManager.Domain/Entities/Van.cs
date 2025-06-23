namespace VanManager.Domain.Entities;

public class Van
{
    public Guid Id { get; set; }
    public string PlateNumber { get; set; } = null!;
    public string? Model { get; set; }
    public string? Brand { get; set; }
    public int Capacity { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public Guid FleetId { get; set; }
    public Guid? DriverId { get; set; }

    // Navigation properties
    public Fleet Fleet { get; set; } = null!;
    public AppUser? Driver { get; set; }
    public ICollection<Route> Routes { get; set; } = new List<Route>();
    public ICollection<Student> Students { get; set; } = new List<Student>();
}