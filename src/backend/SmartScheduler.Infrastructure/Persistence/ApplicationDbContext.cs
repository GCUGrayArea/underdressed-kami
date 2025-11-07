using Microsoft.EntityFrameworkCore;
using SmartScheduler.Domain.Entities;
using SmartScheduler.Infrastructure.Persistence.Configurations;
using SmartScheduler.Infrastructure.Persistence.Seeds;

namespace SmartScheduler.Infrastructure.Persistence;

/// <summary>
/// EF Core database context for SmartScheduler application.
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<JobType> JobTypes => Set<JobType>();
    public DbSet<Contractor> Contractors => Set<Contractor>();
    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<WeeklySchedule> WeeklySchedules => Set<WeeklySchedule>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply entity configurations
        modelBuilder.ApplyConfiguration(new JobTypeConfiguration());
        modelBuilder.ApplyConfiguration(new ContractorConfiguration());
        modelBuilder.ApplyConfiguration(new JobConfiguration());
        modelBuilder.ApplyConfiguration(new WeeklyScheduleConfiguration());

        // Apply seed data
        JobTypeSeed.Seed(modelBuilder);
    }
}
