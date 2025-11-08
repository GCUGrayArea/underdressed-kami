using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartScheduler.Domain.Entities;

namespace SmartScheduler.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core entity configuration for DomainEventLog.
/// </summary>
public class DomainEventLogConfiguration : IEntityTypeConfiguration<DomainEventLog>
{
    public void Configure(EntityTypeBuilder<DomainEventLog> builder)
    {
        builder.ToTable("DomainEventLogs");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.EventId)
            .IsRequired();

        builder.Property(e => e.EventType)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.EventData)
            .IsRequired()
            .HasColumnType("jsonb"); // PostgreSQL JSONB for efficient querying

        builder.Property(e => e.OccurredAt)
            .IsRequired();

        builder.Property(e => e.LoggedAt)
            .IsRequired();

        builder.Property(e => e.UserId)
            .HasMaxLength(100);

        // Indexes for common queries
        builder.HasIndex(e => e.EventId)
            .IsUnique();

        builder.HasIndex(e => e.EventType);

        builder.HasIndex(e => e.OccurredAt);

        builder.HasIndex(e => new { e.EventType, e.OccurredAt });
    }
}
