using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartScheduler.Domain.Entities;

namespace SmartScheduler.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core entity configuration for JobType.
/// </summary>
public class JobTypeConfiguration : IEntityTypeConfiguration<JobType>
{
    public void Configure(EntityTypeBuilder<JobType> builder)
    {
        builder.ToTable("job_types");

        builder.HasKey(jt => jt.Id);

        builder.Property(jt => jt.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(jt => jt.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(jt => jt.Description)
            .HasColumnName("description")
            .HasMaxLength(500);

        builder.Property(jt => jt.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        // Indexes
        builder.HasIndex(jt => jt.Name)
            .IsUnique()
            .HasDatabaseName("ix_job_types_name");

        builder.HasIndex(jt => jt.IsActive)
            .HasDatabaseName("ix_job_types_is_active");
    }
}
