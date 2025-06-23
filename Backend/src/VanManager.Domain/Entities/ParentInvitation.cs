namespace VanManager.Domain.Entities;

public class ParentInvitation
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string InvitationToken { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public bool IsAccepted { get; set; }
    public bool IsValid => !IsAccepted && ExpiresAt > DateTime.UtcNow;
    public Guid StudentId { get; set; }
    public Guid InvitedByUserId { get; set; }
    
    // Navigation properties
    public Student Student { get; set; } = null!;
    public AppUser InvitedBy { get; set; } = null!;
} 