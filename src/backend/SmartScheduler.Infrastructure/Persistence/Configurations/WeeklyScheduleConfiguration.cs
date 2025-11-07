using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartScheduler.Domain.Entities;

namespace SmartScheduler.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core entity configuration for WeeklySchedule.
/// </summary>
public class WeeklyScheduleConfiguration : IEntityTypeConfiguration<WeeklySchedule>
{
    public void Configure(EntityTypeBuilder<WeeklySchedule> builder)
    {
        builder.ToTable("weekly_schedules");

        builder.HasKey(ws => ws.Id);

        builder.Property(ws => ws.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(ws => ws.ContractorId)
            .HasColumnName("contractor_id")
            .IsRequired();

        builder.Property(ws => ws.DayOfWeek)
            .HasColumnName("day_of_week")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(ws => ws.StartTime)
            .HasColumnName("start_time")
            .HasColumnType("time")
            .IsRequired();

        builder.Property(ws => ws.EndTime)
            .HasColumnName("end_time")
            .HasColumnType("time")
            .IsRequired();

        // Indexes
        builder.HasIndex(ws => ws.ContractorId)
            .HasDatabaseName("ix_weekly_schedules_contractor_id");

        builder.HasIndex(ws => new { ws.ContractorId, ws.DayOfWeek })
            .HasDatabaseName("ix_weekly_schedules_contractor_day");

        // Foreign key relationship
        builder.HasOne<Contractor>()
            .WithMany()
            .HasForeignKey(ws => ws.ContractorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
