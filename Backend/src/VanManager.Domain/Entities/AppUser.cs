using Microsoft.AspNetCore.Identity;

namespace VanManager.Domain.Entities;

public class AppUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public String Document { get; set; } = string.Empty;
    
    // Navigation properties
    public ICollection<Fleet> OwnedFleets { get; set; } = new List<Fleet>();    
    public ICollection<TripCheckout> ResponsibleCheckouts { get; set; } = new List<TripCheckout>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<Student> Children { get; set; } = new List<Student>();
}