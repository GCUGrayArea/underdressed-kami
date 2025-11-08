using Microsoft.EntityFrameworkCore;
using SmartScheduler.Domain.Entities;
using SmartScheduler.Domain.ValueObjects;
using SmartScheduler.Infrastructure.Persistence;

namespace SmartScheduler.IntegrationTests;

/// <summary>
/// Helper class for seeding test data into the database.
/// Provides methods to create contractors, jobs, and schedules.
/// </summary>
public class TestDataSeeder
{
    private readonly ApplicationDbContext _context;
    private int _contractorCounter = 1;
    private int _jobCounter = 1;

    public TestDataSeeder(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Creates a contractor with working hours and saves to database.
    /// </summary>
    public async Task<Contractor> CreateContractorAsync(
        Guid jobTypeId,
        string name,
        double latitude,
        double longitude,
        double rating = 4.5,
        List<(DayOfWeek day, TimeOnly start, TimeOnly end)>? schedule = null)
    {
        var contractor = new Contractor(
            formattedId: $"CTR-{_contractorCounter++:D3}",
            name: name,
            jobTypeId: jobTypeId,
            baseLocation: new Location(latitude, longitude),
            phone: "555-0100",
            email: $"contractor{_contractorCounter}@test.com",
            rating: (decimal)rating);

        _context.Contractors.Add(contractor);
        await _context.SaveChangesAsync();

        // Add working hours if provided
        if (schedule != null)
        {
            foreach (var (day, start, end) in schedule)
            {
                var weeklySchedule = new WeeklySchedule(
                    contractor.Id,
                    day,
                    start,
                    end);

                _context.WeeklySchedules.Add(weeklySchedule);
            }

            await _context.SaveChangesAsync();
        }

        return contractor;
    }

    /// <summary>
    /// Creates a standard 9-5 weekday schedule for a contractor.
    /// </summary>
    public List<(DayOfWeek day, TimeOnly start, TimeOnly end)>
        CreateWeekdaySchedule()
    {
        return new List<(DayOfWeek, TimeOnly, TimeOnly)>
        {
            (DayOfWeek.Monday, new TimeOnly(9, 0), new TimeOnly(17, 0)),
            (DayOfWeek.Tuesday, new TimeOnly(9, 0), new TimeOnly(17, 0)),
            (DayOfWeek.Wednesday, new TimeOnly(9, 0), new TimeOnly(17, 0)),
            (DayOfWeek.Thursday, new TimeOnly(9, 0), new TimeOnly(17, 0)),
            (DayOfWeek.Friday, new TimeOnly(9, 0), new TimeOnly(17, 0))
        };
    }

    /// <summary>
    /// Creates a custom schedule for specific days.
    /// </summary>
    public List<(DayOfWeek day, TimeOnly start, TimeOnly end)>
        CreateCustomSchedule(
            DayOfWeek day,
            int startHour,
            int endHour)
    {
        return new List<(DayOfWeek, TimeOnly, TimeOnly)>
        {
            (day, new TimeOnly(startHour, 0), new TimeOnly(endHour, 0))
        };
    }

    /// <summary>
    /// Creates a job and saves to database.
    /// </summary>
    public async Task<Job> CreateJobAsync(
        Guid jobTypeId,
        double latitude,
        double longitude,
        DateOnly desiredDate,
        TimeOnly desiredTime,
        double estimatedDurationHours,
        string customerName = "Test Customer")
    {
        var job = new Job(
            formattedId: $"JOB-{_jobCounter++:D3}",
            jobTypeId: jobTypeId,
            customerId: $"CUST-{_jobCounter:D3}",
            customerName: customerName,
            location: new Location(latitude, longitude),
            desiredDate: DateTime.SpecifyKind(
                desiredDate.ToDateTime(TimeOnly.MinValue),
                DateTimeKind.Utc),
            estimatedDurationHours: (decimal)estimatedDurationHours,
            desiredTime: desiredTime);

        _context.Jobs.Add(job);
        await _context.SaveChangesAsync();

        return job;
    }

    /// <summary>
    /// Assigns a contractor to a job and saves to database.
    /// </summary>
    public async Task AssignJobAsync(
        Job job,
        Guid contractorId,
        DateTime scheduledStartTime)
    {
        var utcTime = DateTime.SpecifyKind(scheduledStartTime, DateTimeKind.Utc);
        job.AssignToContractor(contractorId, utcTime);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Gets the first job type (Tile Installer) for testing.
    /// </summary>
    public async Task<Guid> GetTileInstallerJobTypeIdAsync()
    {
        var jobType = await _context.JobTypes
            .FirstAsync(jt => jt.Name == "Tile Installer");
        return jobType.Id;
    }

    /// <summary>
    /// Clears all test data from the database.
    /// </summary>
    public async Task ClearAllDataAsync()
    {
        _context.Jobs.RemoveRange(_context.Jobs);
        _context.WeeklySchedules.RemoveRange(_context.WeeklySchedules);
        _context.Contractors.RemoveRange(_context.Contractors);
        await _context.SaveChangesAsync();
    }
}
