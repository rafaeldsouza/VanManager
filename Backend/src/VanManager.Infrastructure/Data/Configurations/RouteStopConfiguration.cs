using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VanManager.Domain.Entities;

namespace VanManager.Infrastructure.Data.Configurations;

public class RouteStopConfiguration : IEntityTypeConfiguration<RouteStop>
{
    public void Configure(EntityTypeBuilder<RouteStop> builder)
    {
        // Configuração do Type
        builder.Property(e => e.Type)
            .IsRequired()
            .HasMaxLength(20);

        // Configuração das coordenadas
        builder.Property(e => e.LocationLat)
            .HasPrecision(10, 6);

        builder.Property(e => e.LocationLng)
            .HasPrecision(10, 6);

        // Relacionamento com Route
        builder.HasOne(e => e.Route)
            .WithMany(r => r.Stops)
            .HasForeignKey(e => e.RouteId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relacionamento com Student
        builder.HasOne(e => e.Student)
            .WithMany()
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices
        builder.HasIndex(e => e.RouteId);
        builder.HasIndex(e => e.StudentId);
        builder.HasIndex(e => e.Timestamp);
        builder.HasIndex(e => e.Type);
    }
}
