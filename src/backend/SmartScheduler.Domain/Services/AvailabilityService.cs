using SmartScheduler.Domain.Entities;
using SmartScheduler.Domain.ValueObjects;

namespace SmartScheduler.Domain.Services;

/// <summary>
/// Domain service that calculates contractor availability based on
/// working hours and existing job assignments.
/// </summary>
public class AvailabilityService : IAvailabilityService
{
    /// <summary>
    /// Main orchestration method that calculates available time slots.
    /// </summary>
    public IEnumerable<TimeSlot> CalculateAvailability(
        IEnumerable<WeeklySchedule> workingHours,
        IEnumerable<Job> existingJobs,
        double requiredDurationHours)
    {
        if (workingHours == null)
            throw new ArgumentNullException(nameof(workingHours));

        if (existingJobs == null)
            throw new ArgumentNullException(nameof(existingJobs));

        var workingHoursList = workingHours.ToList();
        var existingJobsList = existingJobs.ToList();

        // If contractor not working this day, return empty
        if (!workingHoursList.Any())
            return Enumerable.Empty<TimeSlot>();

        // Get time slots from working hours
        var workingSlots = GetWorkingSlotsFromSchedule(workingHoursList);

        // Get time slots occupied by existing jobs
        var occupiedSlots = GetOccupiedTimeSlots(existingJobsList);

        // Find available gaps
        var availableSlots = FindAvailableGaps(
            workingSlots,
            occupiedSlots,
            requiredDurationHours);

        return availableSlots;
    }

    /// <summary>
    /// Converts working hour schedules into time slots.
    /// </summary>
    private List<TimeSlot> GetWorkingSlotsFromSchedule(
        List<WeeklySchedule> schedules)
    {
        return schedules
            .Select(s => new TimeSlot(s.StartTime, s.EndTime))
            .OrderBy(slot => slot.Start)
            .ToList();
    }

    /// <summary>
    /// Calculates time slots occupied by existing jobs.
    /// Only considers Assigned and InProgress jobs.
    /// </summary>
    private List<TimeSlot> GetOccupiedTimeSlots(List<Job> jobs)
    {
        var occupiedSlots = new List<TimeSlot>();

        var relevantJobs = jobs
            .Where(j => j.Status == JobStatus.Assigned ||
                       j.Status == JobStatus.InProgress)
            .Where(j => j.ScheduledStartTime.HasValue)
            .ToList();

        foreach (var job in relevantJobs)
        {
            var startTime = TimeOnly.FromDateTime(job.ScheduledStartTime!.Value);
            var endTime = startTime.AddHours((double)job.EstimatedDurationHours);
            occupiedSlots.Add(new TimeSlot(startTime, endTime));
        }

        return MergeOverlappingSlots(occupiedSlots);
    }

    /// <summary>
    /// Finds available gaps in working hours that can accommodate required duration.
    /// </summary>
    private List<TimeSlot> FindAvailableGaps(
        List<TimeSlot> workingSlots,
        List<TimeSlot> occupiedSlots,
        double requiredDurationHours)
    {
        var availableSlots = new List<TimeSlot>();

        foreach (var workSlot in workingSlots)
        {
            var gaps = SubtractOccupiedFromWorking(workSlot, occupiedSlots);

            // Filter gaps that can accommodate required duration
            var validGaps = gaps
                .Where(gap => gap.CanAccommodate(requiredDurationHours))
                .ToList();

            availableSlots.AddRange(validGaps);
        }

        return availableSlots;
    }

    /// <summary>
    /// Subtracts occupied time slots from a working time slot,
    /// returning the remaining available gaps.
    /// </summary>
    private List<TimeSlot> SubtractOccupiedFromWorking(
        TimeSlot workSlot,
        List<TimeSlot> occupiedSlots)
    {
        var gaps = new List<TimeSlot> { workSlot };

        foreach (var occupied in occupiedSlots.OrderBy(s => s.Start))
        {
            var newGaps = new List<TimeSlot>();

            foreach (var gap in gaps)
            {
                if (!gap.Overlaps(occupied))
                {
                    // No overlap, keep the gap
                    newGaps.Add(gap);
                }
                else
                {
                    // Split the gap around occupied slot
                    if (gap.Start < occupied.Start)
                    {
                        // Add gap before occupied slot
                        newGaps.Add(new TimeSlot(gap.Start, occupied.Start));
                    }

                    if (gap.End > occupied.End)
                    {
                        // Add gap after occupied slot
                        newGaps.Add(new TimeSlot(occupied.End, gap.End));
                    }
                }
            }

            gaps = newGaps;
        }

        return gaps;
    }

    /// <summary>
    /// Merges overlapping time slots to handle data errors gracefully.
    /// </summary>
    private List<TimeSlot> MergeOverlappingSlots(List<TimeSlot> slots)
    {
        if (!slots.Any())
            return new List<TimeSlot>();

        var sorted = slots.OrderBy(s => s.Start).ToList();
        var merged = new List<TimeSlot> { sorted[0] };

        for (int i = 1; i < sorted.Count; i++)
        {
            var current = sorted[i];
            var last = merged[merged.Count - 1];

            if (current.Start <= last.End)
            {
                // Overlapping - merge them
                var newEnd = current.End > last.End ? current.End : last.End;
                merged[merged.Count - 1] = new TimeSlot(last.Start, newEnd);
            }
            else
            {
                // No overlap - add as new slot
                merged.Add(current);
            }
        }

        return merged;
    }
}
