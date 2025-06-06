namespace VanManager.Domain.Entities;

public class Notification
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string NotificationType { get; set; } = string.Empty; // Push, SMS, Email
    public string Message { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // Enviada, Falha, Confirmada
    public DateTime Timestamp { get; set; }
    
    // Navigation properties
    public AppUser User { get; set; } = null!;
}