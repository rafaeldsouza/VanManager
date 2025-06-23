using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VanManager.Domain.Entities;

namespace VanManager.Infrastructure.Data.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.Property(e => e.FullName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.DateOfBirth)
            .IsRequired();

        builder.Property(e => e.Document)
            .HasMaxLength(30);

        builder.Property(e => e.IsActive)
            .IsRequired();
        builder.Property(e => e.CreatedAt)
            .IsRequired();
        builder.Property(e => e.UpdatedAt);

        builder.Property(e => e.VanId);
        builder.Property(e => e.FleetId)
            .IsRequired();
        builder.Property(e => e.SchoolId)
            .IsRequired();
        builder.Property(e => e.ParentId)
            .IsRequired();
        builder.Property(e => e.PhoneNumber)
            .HasMaxLength(20)
            .IsRequired();
        builder.Property(e => e.Address)
            .HasMaxLength(200)
            .IsRequired();
        builder.Property(e => e.Email)
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(e => e.ProfilePictureUrl)
            .HasMaxLength(200);
        builder.Property(e => e.Notes)
            .HasMaxLength(500);

        builder.HasOne(e => e.Fleet)
            .WithMany()
            .HasForeignKey(e => e.FleetId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.Guardians)
            .WithMany()
            .UsingEntity(j => j.ToTable("StudentGuardians"));

        // Índices
        builder.HasIndex(e => e.FleetId);
        builder.HasIndex(e => e.Document);
        builder.HasIndex(e => e.Email);
        builder.HasIndex(e => e.IsActive);
    }
}