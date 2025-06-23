using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VanManager.Domain.Entities;

namespace VanManager.Infrastructure.Data.Configurations;

public class FleetConfiguration : IEntityTypeConfiguration<Fleet>
{
    public void Configure(EntityTypeBuilder<Fleet> builder)
    {
        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Description)
            .HasMaxLength(500);

        builder.Property(e => e.LogoUrl)
            .HasMaxLength(200);

        builder.Property(e => e.Address)
            .HasMaxLength(200);

        builder.Property(e => e.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(e => e.Email)
            .HasMaxLength(100);

        builder.Property(e => e.Website)
            .HasMaxLength(100);

        builder.Property(e => e.IsActive)
            .IsRequired();
        builder.Property(e => e.CreatedAt)
            .IsRequired();
        builder.Property(e => e.UpdatedAt);

        builder.HasOne(e => e.Owner)
            .WithMany(e => e.OwnedFleets)
            .HasForeignKey(e => e.OwnerUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índice para busca por owner
        builder.HasIndex(e => e.OwnerUserId);
    }
}