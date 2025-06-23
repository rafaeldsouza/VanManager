using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VanManager.Domain.Entities;

namespace VanManager.Infrastructure.Data.Configurations;

public class StudentTripLogConfiguration : IEntityTypeConfiguration<StudentTripLog>
{
    public void Configure(EntityTypeBuilder<StudentTripLog> builder)
    {
        builder.Property(e => e.BoardingAddress)
            .HasMaxLength(200);

        builder.Property(e => e.DropoffAddress)
            .HasMaxLength(200);

        builder.Property(e => e.Notes)
            .HasMaxLength(500);

        builder.HasOne(e => e.Student)
            .WithMany(e => e.TripLogs)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Route)
            .WithMany()
            .HasForeignKey(e => e.RouteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Van)
            .WithMany()
            .HasForeignKey(e => e.VanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.CreatedBy)
            .WithMany()
            .HasForeignKey(e => e.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.UpdatedBy)
            .WithMany()
            .HasForeignKey(e => e.UpdatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices
        builder.HasIndex(e => e.StudentId);
        builder.HasIndex(e => e.RouteId);
        builder.HasIndex(e => e.VanId);
        builder.HasIndex(e => e.BoardingTime);
        builder.HasIndex(e => e.DropoffTime);
        builder.HasIndex(e => e.Status);
    }
} 