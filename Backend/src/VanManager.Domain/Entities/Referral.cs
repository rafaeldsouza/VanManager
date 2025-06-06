namespace VanManager.Domain.Entities;

public class Referral
{
    public Guid Id { get; set; }
    public Guid ReferrerId { get; set; }  // Quem indicou
    public Guid ReferredId { get; set; }  // Quem foi indicado
    public string Status { get; set; } = string.Empty; // Status da indicação (pendente, ativo)
    
    // Navigation properties
    public AppUser Referrer { get; set; } = null!;
    public AppUser Referred { get; set; } = null!;
}