using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VanManager.Domain.Entities;

namespace VanManager.Infrastructure.Data.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.Property(e => e.Token)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.CreatedByIp)
            .HasMaxLength(50);

        builder.Property(e => e.RevokedByIp)
            .HasMaxLength(50);

        builder.Property(e => e.ReplacedByToken)
            .HasMaxLength(100);

        builder.Property(e => e.ReasonRevoked)
            .HasMaxLength(200);

        builder.Property(e => e.CreatedAt)
            .IsRequired();
        builder.Property(e => e.RevokedAt);
        builder.Property(e => e.ExpiresAt)
            .IsRequired();

        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índices
        builder.HasIndex(e => e.Token);
        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => e.ExpiresAt);
    }
}