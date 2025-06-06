namespace VanManager.Domain.Entities;

public class AuthorizedGuardian
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }

    public string FullName { get; set; } = string.Empty;
    public string Relationship { get; set; } = string.Empty; // Ex: Pai, Avó, Tio
    public string PhoneNumber { get; set; } = string.Empty;  // Opcional, se quiser contato direto
    public string DocumentId { get; set; } = string.Empty;   // RG, CPF ou outro identificador
    public string Description { get; set; } = string.Empty;  // Ex: "Busca às terças", "Autorizado sempre"

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    public Student Student { get; set; } = null!;
}