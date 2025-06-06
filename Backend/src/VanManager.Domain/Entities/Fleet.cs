namespace VanManager.Domain.Entities;

public class Fleet
{
    public Guid Id { get; set; }
    public Guid OwnerUserId { get; set; }
    public string Name { get; set; } = string.Empty;
    
    // Navigation properties
    public AppUser Owner { get; set; } = null!;
    public ICollection<Van> Vans { get; set; } = new List<Van>();
}