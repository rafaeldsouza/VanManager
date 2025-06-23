using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VanManager.Domain.Entities;

namespace VanManager.Infrastructure.Data.Configurations;

public class FleetSubscriptionConfiguration : IEntityTypeConfiguration<FleetSubscription>
{
    public void Configure(EntityTypeBuilder<FleetSubscription> builder)
    {
        builder.Property(e => e.PaymentMethod)
            .HasMaxLength(50);

        builder.Property(e => e.TransactionId)
            .HasMaxLength(100);

        builder.Property(e => e.Notes)
            .HasMaxLength(500);

        builder.Property(e => e.IsActive)
            .IsRequired();
        builder.Property(e => e.CreatedAt)
            .IsRequired();
        builder.Property(e => e.UpdatedAt);
        builder.Property(e => e.CancellationReason)
            .HasMaxLength(200);
        builder.Property(e => e.CancelledAt);

        builder.HasOne(e => e.Fleet)
            .WithMany(e => e.Subscriptions)
            .HasForeignKey(e => e.FleetId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Plan)
            .WithMany()
            .HasForeignKey(e => e.PlanId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices
        builder.HasIndex(e => e.FleetId);
        builder.HasIndex(e => e.PlanId);
        builder.HasIndex(e => e.StartDate);
        builder.HasIndex(e => e.EndDate);
    }
}