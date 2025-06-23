using Microsoft.AspNetCore.Identity;

namespace VanManager.Domain.Entities;

public class AppUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = null!;
    public string? Document { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public Guid? FleetId { get; set; }
    public Guid? VanId { get; set; }

    // Navigation properties
    public Fleet? Fleet { get; set; }
    public Van? Van { get; set; }
    public ICollection<Fleet> OwnedFleets { get; set; } = new List<Fleet>();
    public ICollection<TripCheckout> ResponsibleCheckouts { get; set; } = new List<TripCheckout>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<Student> Children { get; set; } = new List<Student>();
    public ICollection<FleetSubscription> FleetSubscriptions { get; set; } = new List<FleetSubscription>();
    public ICollection<IdentityUserRole<Guid>> UserRoles { get; set; } = new List<IdentityUserRole<Guid>>();
}