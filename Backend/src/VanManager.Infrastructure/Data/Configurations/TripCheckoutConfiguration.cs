using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VanManager.Domain.Entities;

namespace VanManager.Infrastructure.Data.Configurations;

public class TripCheckoutConfiguration : IEntityTypeConfiguration<TripCheckout>
{
    public void Configure(EntityTypeBuilder<TripCheckout> builder)
    {        builder.Property(e => e.Timestamp)
            .IsRequired()
            .HasColumnType("timestamp without time zone");

        builder.HasOne(e => e.Student)
            .WithMany(s => s.Checkouts)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Responsible)
            .WithMany(u => u.ResponsibleCheckouts)
            .HasForeignKey(e => e.ResponsibleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Route)
            .WithMany(r => r.Checkouts)
            .HasForeignKey(e => e.RouteId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices
        builder.HasIndex(e => e.StudentId);
        builder.HasIndex(e => e.ResponsibleId);
        builder.HasIndex(e => e.RouteId);
        builder.HasIndex(e => e.Timestamp);
    }
} 