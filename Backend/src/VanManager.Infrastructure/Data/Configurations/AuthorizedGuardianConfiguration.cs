using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VanManager.Domain.Entities;

namespace VanManager.Infrastructure.Data.Configurations;

public class AuthorizedGuardianConfiguration : IEntityTypeConfiguration<AuthorizedGuardian>
{
    public void Configure(EntityTypeBuilder<AuthorizedGuardian> builder)
    {
        builder.Property(e => e.FullName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.DocumentId)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(e => e.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(e => e.Relationship)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Description)
            .HasMaxLength(200);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.HasOne(e => e.Student)
            .WithMany() // Student não tem AuthorizedGuardians
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índices
        builder.HasIndex(e => e.DocumentId);
        builder.HasIndex(e => e.StudentId);
    }
}