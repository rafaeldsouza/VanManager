using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VanManager.Domain.Entities;

namespace VanManager.Infrastructure.Data.Configurations;

public class ReferralConfiguration : IEntityTypeConfiguration<Referral>
{
    public void Configure(EntityTypeBuilder<Referral> builder)
    {
        builder.Property(e => e.ReferralCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(e => e.Notes)
            .HasMaxLength(500);

        builder.HasOne(e => e.Referrer)
            .WithMany()
            .HasForeignKey(e => e.ReferrerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Referred)
            .WithMany()
            .HasForeignKey(e => e.ReferredId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices
        builder.HasIndex(e => e.ReferralCode);
        builder.HasIndex(e => e.ReferrerId);
        builder.HasIndex(e => e.ReferredId);
        builder.HasIndex(e => e.IsCompleted);
    }
} 