using SmartScheduler.Domain.Entities;
using SmartScheduler.Domain.ValueObjects;

namespace SmartScheduler.Tests.TestHelpers;

/// <summary>
/// Builder pattern for creating test data with fluent API.
/// Provides convenient methods for constructing WeeklySchedule and Job entities.
/// </summary>
public static class TestDataBuilder
{
    public static class Schedules
    {
        /// <summary>
        /// Creates a standard 9-5 weekday schedule for a contractor.
        /// </summary>
        public static WeeklySchedule StandardWorkDay(
            Guid contractorId,
            DayOfWeek dayOfWeek)
        {
            return new WeeklySchedule(
                contractorId,
                dayOfWeek,
                new TimeOnly(9, 0),
                new TimeOnly(17, 0));
        }

        /// <summary>
        /// Creates a custom schedule for a contractor.
        /// </summary>
        public static WeeklySchedule Custom(
            Guid contractorId,
            DayOfWeek dayOfWeek,
            int startHour,
            int startMinute,
            int endHour,
            int endMinute)
        {
            return new WeeklySchedule(
                contractorId,
                dayOfWeek,
                new TimeOnly(startHour, startMinute),
                new TimeOnly(endHour, endMinute));
        }

        /// <summary>
        /// Creates a split shift schedule (morning and afternoon).
        /// </summary>
        public static List<WeeklySchedule> SplitShift(
            Guid contractorId,
            DayOfWeek dayOfWeek,
            int morningStart,
            int morningEnd,
            int afternoonStart,
            int afternoonEnd)
        {
            return new List<WeeklySchedule>
            {
                Custom(contractorId, dayOfWeek, morningStart, 0, morningEnd, 0),
                Custom(contractorId, dayOfWeek, afternoonStart, 0, afternoonEnd, 0)
            };
        }
    }

    public static class Jobs
    {
        private static int _jobCounter = 1;

        /// <summary>
        /// Creates a job with specified start time and duration.
        /// </summary>
        public static Job WithSchedule(
            Guid contractorId,
            DateTime scheduledStartTime,
            double durationHours,
            JobStatus status = JobStatus.Assigned)
        {
            var job = new Job(
                formattedId: $"JOB-{_jobCounter++:D3}",
                jobTypeId: Guid.NewGuid(),
                customerId: "CUST-001",
                customerName: "Test Customer",
                location: new Location(40.7128, -74.0060),
                desiredDate: scheduledStartTime.Date,
                estimatedDurationHours: (decimal)durationHours);

            if (status == JobStatus.Assigned || status == JobStatus.InProgress)
            {
                job.AssignToContractor(contractorId, scheduledStartTime);

                if (status == JobStatus.InProgress)
                {
                    job.MarkInProgress();
                }
            }

            return job;
        }

        /// <summary>
        /// Creates an assigned job with time specified as hours and minutes.
        /// </summary>
        public static Job AssignedAt(
            Guid contractorId,
            DateTime date,
            int startHour,
            int startMinute,
            double durationHours)
        {
            var scheduledStart = new DateTime(
                date.Year,
                date.Month,
                date.Day,
                startHour,
                startMinute,
                0);

            return WithSchedule(contractorId, scheduledStart, durationHours);
        }

        /// <summary>
        /// Creates an unassigned job (doesn't occupy any time slots).
        /// </summary>
        public static Job Unassigned(DateTime desiredDate, double durationHours)
        {
            return new Job(
                formattedId: $"JOB-{_jobCounter++:D3}",
                jobTypeId: Guid.NewGuid(),
                customerId: "CUST-001",
                customerName: "Test Customer",
                location: new Location(40.7128, -74.0060),
                desiredDate: desiredDate,
                estimatedDurationHours: (decimal)durationHours);
        }

        /// <summary>
        /// Resets the job counter for test isolation.
        /// </summary>
        public static void ResetCounter()
        {
            _jobCounter = 1;
        }
    }
}
