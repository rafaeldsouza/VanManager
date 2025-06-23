using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VanManager.Domain.Entities;

namespace VanManager.Infrastructure.Data.Configurations;

public class VanConfiguration : IEntityTypeConfiguration<Van>
{
    public void Configure(EntityTypeBuilder<Van> builder)
    {
        builder.Property(e => e.PlateNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(e => e.Model)
            .HasMaxLength(50);

        builder.Property(e => e.Brand)
            .HasMaxLength(50);


        builder.Property(e => e.Capacity)
            .IsRequired();

        builder.Property(e => e.IsActive)
            .IsRequired();
        builder.Property(e => e.CreatedAt)
            .IsRequired();
        builder.Property(e => e.UpdatedAt);

        builder.HasOne(e => e.Fleet)
            .WithMany(e => e.Vans)
            .HasForeignKey(e => e.FleetId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Driver)
            .WithMany()
            .HasForeignKey(e => e.DriverId)
            .OnDelete(DeleteBehavior.SetNull);

        // Índices
        builder.HasIndex(e => e.PlateNumber);
        builder.HasIndex(e => e.FleetId);
        builder.HasIndex(e => e.DriverId);
        builder.HasIndex(e => e.IsActive);
    }
}