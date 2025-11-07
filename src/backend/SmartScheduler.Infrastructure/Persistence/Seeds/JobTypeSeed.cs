using Microsoft.EntityFrameworkCore;
using SmartScheduler.Domain.Entities;

namespace SmartScheduler.Infrastructure.Persistence.Seeds;

/// <summary>
/// Seeds initial job types for the flooring industry.
/// </summary>
public static class JobTypeSeed
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        var tileInstallerId = Guid.Parse("10000000-0000-0000-0000-000000000001");
        var carpetInstallerId = Guid.Parse("10000000-0000-0000-0000-000000000002");
        var hardwoodSpecialistId = Guid.Parse("10000000-0000-0000-0000-000000000003");

        modelBuilder.Entity<JobType>().HasData(
            new
            {
                Id = tileInstallerId,
                Name = "Tile Installer",
                Description = "Specializes in ceramic, porcelain, and natural stone tile installation",
                IsActive = true
            },
            new
            {
                Id = carpetInstallerId,
                Name = "Carpet Installer",
                Description = "Specializes in carpet installation and replacement",
                IsActive = true
            },
            new
            {
                Id = hardwoodSpecialistId,
                Name = "Hardwood Specialist",
                Description = "Specializes in hardwood floor installation, refinishing, and repair",
                IsActive = true
            }
        );
    }
}
