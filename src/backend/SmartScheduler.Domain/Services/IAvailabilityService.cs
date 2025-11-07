using SmartScheduler.Domain.Entities;
using SmartScheduler.Domain.ValueObjects;

namespace SmartScheduler.Domain.Services;

/// <summary>
/// Domain service for calculating contractor availability based on
/// working hours and existing job assignments.
/// </summary>
public interface IAvailabilityService
{
    /// <summary>
    /// Calculates available time slots for a contractor on a specific date.
    /// Takes into account contractor working hours and existing job assignments.
    /// </summary>
    /// <param name="workingHours">The contractor's working hours for the target date</param>
    /// <param name="existingJobs">Jobs already assigned to the contractor for that date</param>
    /// <param name="requiredDurationHours">Minimum duration required for new job</param>
    /// <returns>List of available time slots that can accommodate the required duration</returns>
    IEnumerable<TimeSlot> CalculateAvailability(
        IEnumerable<WeeklySchedule> workingHours,
        IEnumerable<Job> existingJobs,
        double requiredDurationHours);
}
