using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartScheduler.Domain.Entities;

namespace SmartScheduler.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core entity configuration for Contractor.
/// </summary>
public class ContractorConfiguration : IEntityTypeConfiguration<Contractor>
{
    public void Configure(EntityTypeBuilder<Contractor> builder)
    {
        builder.ToTable("contractors");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(c => c.FormattedId)
            .HasColumnName("formatted_id")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(c => c.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.JobTypeId)
            .HasColumnName("job_type_id")
            .IsRequired();

        builder.Property(c => c.Rating)
            .HasColumnName("rating")
            .HasPrecision(3, 1)
            .IsRequired();

        builder.Property(c => c.Phone)
            .HasColumnName("phone")
            .HasMaxLength(20);

        builder.Property(c => c.Email)
            .HasColumnName("email")
            .HasMaxLength(100);

        builder.Property(c => c.IsActive)
            .HasColumnName("is_active")
            .IsRequired();

        // Location value object as owned entity
        builder.OwnsOne(c => c.BaseLocation, location =>
        {
            location.Property(l => l.Latitude)
                .HasColumnName("base_latitude")
                .HasPrecision(9, 6)
                .IsRequired();

            location.Property(l => l.Longitude)
                .HasColumnName("base_longitude")
                .HasPrecision(9, 6)
                .IsRequired();

            location.Property(l => l.Address)
                .HasColumnName("base_address")
                .HasMaxLength(500);
        });

        // Indexes
        builder.HasIndex(c => c.FormattedId)
            .IsUnique()
            .HasDatabaseName("ix_contractors_formatted_id");

        builder.HasIndex(c => c.JobTypeId)
            .HasDatabaseName("ix_contractors_job_type_id");

        builder.HasIndex(c => c.IsActive)
            .HasDatabaseName("ix_contractors_is_active");

        builder.HasIndex(c => c.Rating)
            .HasDatabaseName("ix_contractors_rating");

        // Foreign key relationship
        builder.HasOne<JobType>()
            .WithMany()
            .HasForeignKey(c => c.JobTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
