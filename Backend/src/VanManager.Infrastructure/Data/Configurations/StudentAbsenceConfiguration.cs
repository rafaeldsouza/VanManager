using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VanManager.Domain.Entities;

namespace VanManager.Infrastructure.Data.Configurations;

public class StudentAbsenceConfiguration : IEntityTypeConfiguration<StudentAbsence>
{
    public void Configure(EntityTypeBuilder<StudentAbsence> builder)
    {
        builder.Property(e => e.Reason)
            .HasMaxLength(500);

        builder.Property(e => e.Justification)
            .HasMaxLength(500);

        builder.HasOne(e => e.Student)
            .WithMany(e => e.Absences)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

       /* builder.HasOne(e => e.JustifiedBy)
            .WithMany()
            .HasForeignKey(e => e.JustifiedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.ApprovedBy)
            .WithMany()
            .HasForeignKey(e => e.ApprovedByUserId)
            .OnDelete(DeleteBehavior.Restrict);*/

        // Índices
        builder.HasIndex(e => e.StudentId);
        builder.HasIndex(e => e.Date);
        builder.HasIndex(e => e.IsJustified);
    }
} 