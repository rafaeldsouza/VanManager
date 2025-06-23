using System.ComponentModel.DataAnnotations;

namespace VanManager.Domain.Entities;

public class Fleet
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public Guid OwnerUserId { get; set; }
    public string? LogoUrl { get; set; }
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    
    // Navigation properties
    public AppUser Owner { get; set; } = null!;
    public ICollection<Van> Vans { get; set; } = new List<Van>();
    public ICollection<FleetSubscription> Subscriptions { get; set; } = new List<FleetSubscription>();
    
    // Helper property to get active subscription
    public FleetSubscription? Subscription => Subscriptions
        .FirstOrDefault(s => s.IsActive && s.EndDate > DateTime.UtcNow);
}