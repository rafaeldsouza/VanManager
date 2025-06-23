using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VanManager.Domain.Entities;

namespace VanManager.Infrastructure.Data.Configurations;

public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.Property(e => e.FullName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Document)
            .HasMaxLength(30);

        builder.Property(e => e.ProfilePictureUrl)
            .HasMaxLength(200);

        builder.Property(e => e.IsActive)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.LastLoginAt);

        builder.Property(e => e.FleetId);
        builder.Property(e => e.VanId);

        // Índices para busca de usuários
        builder.HasIndex(e => e.IsActive);
        builder.HasIndex(e => e.Email);
        builder.HasIndex(e => e.Document);

        builder.HasMany(u => u.Children)
            .WithOne(s => s.Parent)
            .HasForeignKey(s => s.ParentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}