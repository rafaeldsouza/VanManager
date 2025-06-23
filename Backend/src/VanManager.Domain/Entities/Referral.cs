namespace VanManager.Domain.Entities;

public class Referral
{
    public Guid Id { get; set; }
    public Guid ReferrerId { get; set; }  // Quem indicou
    public Guid ReferredId { get; set; }  // Quem foi indicado
    public string ReferralCode { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
    public bool IsCompleted { get; set; }
    public string? Notes { get; set; }
    
    // Navigation properties
    public AppUser Referrer { get; set; } = null!;
    public AppUser Referred { get; set; } = null!;
}