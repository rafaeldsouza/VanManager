namespace VanManager.Domain.Entities;

public class StudentAbsence
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public Guid RouteId { get; set; }
    public DateTime Date { get; set; }
    public string Type { get; set; } = string.Empty; // IDA, VOLTA ou AMBOS
    public string Reason { get; set; } = string.Empty;
    public string? Justification { get; set; }
    public bool IsJustified { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public Guid? UpdatedByUserId { get; set; }
    
    // Navigation properties
    public Student Student { get; set; } = null!;
    public Route Route { get; set; } = null!;
    public AppUser? CreatedBy { get; set; }
    public AppUser? UpdatedBy { get; set; }
}