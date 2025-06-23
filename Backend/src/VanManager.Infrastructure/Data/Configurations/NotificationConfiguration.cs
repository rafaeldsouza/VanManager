using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VanManager.Domain.Entities;

namespace VanManager.Infrastructure.Data.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.Property(e => e.NotificationType)
            .HasMaxLength(20)
            .IsRequired();
        builder.Property(e => e.Message)
            .HasMaxLength(500)
            .IsRequired();
        builder.Property(e => e.Status)
            .HasMaxLength(20)
            .IsRequired();
        builder.Property(e => e.Timestamp)
            .IsRequired();
        builder.Property(e => e.Title)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasOne(e => e.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => e.Timestamp);
    }
}