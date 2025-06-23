using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VanManager.Domain.Entities;

namespace VanManager.Infrastructure.Data.Configurations;

public class ParentInvitationConfiguration : IEntityTypeConfiguration<ParentInvitation>
{
    public void Configure(EntityTypeBuilder<ParentInvitation> builder)
    {
        builder.Property(e => e.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.InvitationToken)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne(e => e.Student)
            .WithMany(e => e.ParentInvitations)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.InvitedBy)
            .WithMany()
            .HasForeignKey(e => e.InvitedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices
        builder.HasIndex(e => e.Email);
        builder.HasIndex(e => e.InvitationToken);
        builder.HasIndex(e => e.StudentId);
        builder.HasIndex(e => e.ExpiresAt);
        builder.HasIndex(e => e.IsAccepted);
    }
} 