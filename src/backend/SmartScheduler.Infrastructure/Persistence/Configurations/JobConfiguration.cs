using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartScheduler.Domain.Entities;

namespace SmartScheduler.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core entity configuration for Job.
/// </summary>
public class JobConfiguration : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.ToTable("jobs");

        builder.HasKey(j => j.Id);

        builder.Property(j => j.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(j => j.FormattedId)
            .HasColumnName("formatted_id")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(j => j.JobTypeId)
            .HasColumnName("job_type_id")
            .IsRequired();

        builder.Property(j => j.CustomerId)
            .HasColumnName("customer_id")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(j => j.CustomerName)
            .HasColumnName("customer_name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(j => j.DesiredDate)
            .HasColumnName("desired_date")
            .HasColumnType("date")
            .IsRequired();

        builder.Property(j => j.DesiredTime)
            .HasColumnName("desired_time")
            .HasColumnType("time");

        builder.Property(j => j.EstimatedDurationHours)
            .HasColumnName("estimated_duration_hours")
            .HasPrecision(5, 2)
            .IsRequired();

        builder.Property(j => j.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(j => j.AssignedContractorId)
            .HasColumnName("assigned_contractor_id");

        builder.Property(j => j.ScheduledStartTime)
            .HasColumnName("scheduled_start_time")
            .HasColumnType("timestamp with time zone");

        builder.Property(j => j.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder.Property(j => j.CompletedAt)
            .HasColumnName("completed_at")
            .HasColumnType("timestamp with time zone");

        // Location value object as owned entity
        builder.OwnsOne(j => j.Location, location =>
        {
            location.Property(l => l.Latitude)
                .HasColumnName("location_latitude")
                .HasPrecision(9, 6)
                .IsRequired();

            location.Property(l => l.Longitude)
                .HasColumnName("location_longitude")
                .HasPrecision(9, 6)
                .IsRequired();

            location.Property(l => l.Address)
                .HasColumnName("location_address")
                .HasMaxLength(500);
        });

        // Indexes
        builder.HasIndex(j => j.FormattedId)
            .IsUnique()
            .HasDatabaseName("ix_jobs_formatted_id");

        builder.HasIndex(j => j.JobTypeId)
            .HasDatabaseName("ix_jobs_job_type_id");

        builder.HasIndex(j => j.Status)
            .HasDatabaseName("ix_jobs_status");

        builder.HasIndex(j => j.DesiredDate)
            .HasDatabaseName("ix_jobs_desired_date");

        builder.HasIndex(j => j.AssignedContractorId)
            .HasDatabaseName("ix_jobs_assigned_contractor_id");

        // Foreign key relationships
        builder.HasOne<JobType>()
            .WithMany()
            .HasForeignKey(j => j.JobTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Contractor>()
            .WithMany()
            .HasForeignKey(j => j.AssignedContractorId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
