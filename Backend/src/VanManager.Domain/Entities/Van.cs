namespace VanManager.Domain.Entities;

public class Van
{
    public Guid Id { get; set; }
    public string PlateNumber { get; set; } = string.Empty;
    public Guid FleetId { get; set; }
    public Guid AssignedDriverId { get; set; }
    public int Capacity { get; set; }
    
    // Navigation properties
    public Fleet Fleet { get; set; } = null!;
    public AppUser AssignedDriver { get; set; } = null!;
    public ICollection<Route> Routes { get; set; } = new List<Route>();
}