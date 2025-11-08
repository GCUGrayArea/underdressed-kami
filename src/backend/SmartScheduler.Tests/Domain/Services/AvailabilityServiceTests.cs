using FluentAssertions;
using SmartScheduler.Domain.Entities;
using SmartScheduler.Domain.Services;
using SmartScheduler.Domain.ValueObjects;
using SmartScheduler.Tests.TestHelpers;

namespace SmartScheduler.Tests.Domain.Services;

/// <summary>
/// Comprehensive unit tests for AvailabilityService covering all scenarios:
/// - Normal availability calculations
/// - No availability cases
/// - Edge cases and boundary conditions
/// - Split shifts handling
/// - Overlapping jobs handling
/// </summary>
public class AvailabilityServiceTests
{
    private readonly AvailabilityService _service;
    private readonly Guid _contractorId;
    private readonly DateTime _targetDate;

    public AvailabilityServiceTests()
    {
        _service = new AvailabilityService();
        _contractorId = Guid.NewGuid();
        _targetDate = new DateTime(2025, 1, 15);
    }

    #region No Existing Jobs Tests

    [Fact]
    public void CalculateAvailability_WithNoJobs_ReturnsFullWorkingHours()
    {
        // Arrange - Standard 9-5 workday, no jobs
        var workingHours = new List<WeeklySchedule>
        {
            TestDataBuilder.Schedules.StandardWorkDay(_contractorId, DayOfWeek.Monday)
        };
        var jobs = new List<Job>();

        // Act
        var result = _service.CalculateAvailability(
            workingHours,
            jobs,
            requiredDurationHours: 2.0);

        // Assert
        result.Should().HaveCount(1);
        result.First().Start.Should().Be(new TimeOnly(9, 0));
        result.First().End.Should().Be(new TimeOnly(17, 0));
        result.First().DurationHours.Should().Be(8.0);
    }

    [Fact]
    public void CalculateAvailability_WithNoJobsAndSplitShift_ReturnsBothPeriods()
    {
        // Arrange - Split shift: 9-12 and 14-17
        var workingHours = TestDataBuilder.Schedules.SplitShift(
            _contractorId,
            DayOfWeek.Monday,
            morningStart: 9,
            morningEnd: 12,
            afternoonStart: 14,
            afternoonEnd: 17);
        var jobs = new List<Job>();

        // Act
        var result = _service.CalculateAvailability(
            workingHours,
            jobs,
            requiredDurationHours: 2.0).ToList();

        // Assert
        result.Should().HaveCount(2);
        result[0].Start.Should().Be(new TimeOnly(9, 0));
        result[0].End.Should().Be(new TimeOnly(12, 0));
        result[1].Start.Should().Be(new TimeOnly(14, 0));
        result[1].End.Should().Be(new TimeOnly(17, 0));
    }

    #endregion

    #region No Availability Tests

    [Fact]
    public void CalculateAvailability_WithNoWorkingHours_ReturnsEmpty()
    {
        // Arrange - Contractor not working on this day
        var workingHours = new List<WeeklySchedule>();
        var jobs = new List<Job>();

        // Act
        var result = _service.CalculateAvailability(
            workingHours,
            jobs,
            requiredDurationHours: 2.0);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void CalculateAvailability_WithBackToBackJobs_ReturnsEmpty()
    {
        // Arrange - Jobs fill entire workday (9-5)
        var workingHours = new List<WeeklySchedule>
        {
            TestDataBuilder.Schedules.StandardWorkDay(_contractorId, DayOfWeek.Monday)
        };
        var jobs = new List<Job>
        {
            TestDataBuilder.Jobs.AssignedAt(_contractorId, _targetDate, 9, 0, 4.0),
            TestDataBuilder.Jobs.AssignedAt(_contractorId, _targetDate, 13, 0, 4.0)
        };

        // Act
        var result = _service.CalculateAvailability(
            workingHours,
            jobs,
            requiredDurationHours: 1.0);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void CalculateAvailability_WithJobDurationExceedingGaps_ReturnsEmpty()
    {
        // Arrange - 1-hour gap between jobs, but need 2 hours
        var workingHours = new List<WeeklySchedule>
        {
            TestDataBuilder.Schedules.StandardWorkDay(_contractorId, DayOfWeek.Monday)
        };
        var jobs = new List<Job>
        {
            TestDataBuilder.Jobs.AssignedAt(_contractorId, _targetDate, 9, 0, 3.0),
            TestDataBuilder.Jobs.AssignedAt(_contractorId, _targetDate, 13, 0, 4.0)
        };

        // Act - Need 2 hours but only 1-hour gap available
        var result = _service.CalculateAvailability(
            workingHours,
            jobs,
            requiredDurationHours: 2.0);

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region Gap Between Jobs Tests

    [Fact]
    public void CalculateAvailability_WithGapBetweenJobs_ReturnsCorrectSlot()
    {
        // Arrange - 2-hour gap between jobs (12-14)
        var workingHours = new List<WeeklySchedule>
        {
            TestDataBuilder.Schedules.StandardWorkDay(_contractorId, DayOfWeek.Monday)
        };
        var jobs = new List<Job>
        {
            TestDataBuilder.Jobs.AssignedAt(_contractorId, _targetDate, 9, 0, 3.0),
            TestDataBuilder.Jobs.AssignedAt(_contractorId, _targetDate, 14, 0, 3.0)
        };

        // Act
        var result = _service.CalculateAvailability(
            workingHours,
            jobs,
            requiredDurationHours: 1.0).ToList();

        // Assert
        result.Should().HaveCount(1);
        result[0].Start.Should().Be(new TimeOnly(12, 0));
        result[0].End.Should().Be(new TimeOnly(14, 0));
        result[0].DurationHours.Should().Be(2.0);
    }

    [Fact]
    public void CalculateAvailability_WithMultipleGaps_ReturnsAllValidSlots()
    {
        // Arrange - Three jobs creating two gaps
        var workingHours = new List<WeeklySchedule>
        {
            TestDataBuilder.Schedules.StandardWorkDay(_contractorId, DayOfWeek.Monday)
        };
        var jobs = new List<Job>
        {
            TestDataBuilder.Jobs.AssignedAt(_contractorId, _targetDate, 10, 0, 2.0),
            TestDataBuilder.Jobs.AssignedAt(_contractorId, _targetDate, 14, 0, 2.0)
        };

        // Act
        var result = _service.CalculateAvailability(
            workingHours,
            jobs,
            requiredDurationHours: 1.0).ToList();

        // Assert - Should have gaps before first job, between jobs, and after
        result.Should().HaveCount(3);
        result[0].Start.Should().Be(new TimeOnly(9, 0));
        result[0].End.Should().Be(new TimeOnly(10, 0));
        result[1].Start.Should().Be(new TimeOnly(12, 0));
        result[1].End.Should().Be(new TimeOnly(14, 0));
        result[2].Start.Should().Be(new TimeOnly(16, 0));
        result[2].End.Should().Be(new TimeOnly(17, 0));
    }

    #endregion

    #region Boundary Condition Tests

    [Fact]
    public void CalculateAvailability_WithJobAtStartOfDay_ReturnsRemaining()
    {
        // Arrange - Job starts exactly at work start time
        var workingHours = new List<WeeklySchedule>
        {
            TestDataBuilder.Schedules.StandardWorkDay(_contractorId, DayOfWeek.Monday)
        };
        var jobs = new List<Job>
        {
            TestDataBuilder.Jobs.AssignedAt(_contractorId, _targetDate, 9, 0, 2.0)
        };

        // Act
        var result = _service.CalculateAvailability(
            workingHours,
            jobs,
            requiredDurationHours: 1.0).ToList();

        // Assert - Should return time from 11:00 to 17:00
        result.Should().HaveCount(1);
        result[0].Start.Should().Be(new TimeOnly(11, 0));
        result[0].End.Should().Be(new TimeOnly(17, 0));
    }

    [Fact]
    public void CalculateAvailability_WithJobAtEndOfDay_ReturnsBeginning()
    {
        // Arrange - Job ends exactly at work end time
        var workingHours = new List<WeeklySchedule>
        {
            TestDataBuilder.Schedules.StandardWorkDay(_contractorId, DayOfWeek.Monday)
        };
        var jobs = new List<Job>
        {
            TestDataBuilder.Jobs.AssignedAt(_contractorId, _targetDate, 15, 0, 2.0)
        };

        // Act
        var result = _service.CalculateAvailability(
            workingHours,
            jobs,
            requiredDurationHours: 1.0).ToList();

        // Assert - Should return time from 9:00 to 15:00
        result.Should().HaveCount(1);
        result[0].Start.Should().Be(new TimeOnly(9, 0));
        result[0].End.Should().Be(new TimeOnly(15, 0));
    }

    [Fact]
    public void CalculateAvailability_WithJobSpanningEntireDay_ReturnsEmpty()
    {
        // Arrange - Job covers entire working hours
        var workingHours = new List<WeeklySchedule>
        {
            TestDataBuilder.Schedules.StandardWorkDay(_contractorId, DayOfWeek.Monday)
        };
        var jobs = new List<Job>
        {
            TestDataBuilder.Jobs.AssignedAt(_contractorId, _targetDate, 9, 0, 8.0)
        };

        // Act
        var result = _service.CalculateAvailability(
            workingHours,
            jobs,
            requiredDurationHours: 1.0);

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region Job Status Filter Tests

    [Fact]
    public void CalculateAvailability_WithUnassignedJob_IgnoresJob()
    {
        // Arrange - Unassigned job should not occupy time
        var workingHours = new List<WeeklySchedule>
        {
            TestDataBuilder.Schedules.StandardWorkDay(_contractorId, DayOfWeek.Monday)
        };
        var jobs = new List<Job>
        {
            TestDataBuilder.Jobs.Unassigned(_targetDate, 3.0)
        };

        // Act
        var result = _service.CalculateAvailability(
            workingHours,
            jobs,
            requiredDurationHours: 2.0);

        // Assert - Should return full working hours
        result.Should().HaveCount(1);
        result.First().DurationHours.Should().Be(8.0);
    }

    [Fact]
    public void CalculateAvailability_WithInProgressJob_ConsidersOccupied()
    {
        // Arrange - InProgress job should occupy time
        var workingHours = new List<WeeklySchedule>
        {
            TestDataBuilder.Schedules.StandardWorkDay(_contractorId, DayOfWeek.Monday)
        };
        var scheduledStart = new DateTime(
            _targetDate.Year, _targetDate.Month, _targetDate.Day, 10, 0, 0);
        var job = TestDataBuilder.Jobs.WithSchedule(
            _contractorId,
            scheduledStart,
            3.0,
            JobStatus.InProgress);
        var jobs = new List<Job> { job };

        // Act
        var result = _service.CalculateAvailability(
            workingHours,
            jobs,
            requiredDurationHours: 1.0).ToList();

        // Assert - Should have gaps before and after
        result.Should().HaveCount(2);
        result[0].End.Should().Be(new TimeOnly(10, 0));
        result[1].Start.Should().Be(new TimeOnly(13, 0));
    }

    [Fact]
    public void CalculateAvailability_WithAssignedJob_ConsidersOccupied()
    {
        // Arrange - Assigned job should occupy time
        var workingHours = new List<WeeklySchedule>
        {
            TestDataBuilder.Schedules.StandardWorkDay(_contractorId, DayOfWeek.Monday)
        };
        var jobs = new List<Job>
        {
            TestDataBuilder.Jobs.AssignedAt(_contractorId, _targetDate, 10, 0, 3.0)
        };

        // Act
        var result = _service.CalculateAvailability(
            workingHours,
            jobs,
            requiredDurationHours: 1.0).ToList();

        // Assert - Should have gaps before and after
        result.Should().HaveCount(2);
        result[0].End.Should().Be(new TimeOnly(10, 0));
        result[1].Start.Should().Be(new TimeOnly(13, 0));
    }

    #endregion

    #region Overlapping Jobs Tests

    [Fact]
    public void CalculateAvailability_WithOverlappingJobs_MergesOccupiedTime()
    {
        // Arrange - Two overlapping jobs (data error scenario)
        var workingHours = new List<WeeklySchedule>
        {
            TestDataBuilder.Schedules.StandardWorkDay(_contractorId, DayOfWeek.Monday)
        };
        var jobs = new List<Job>
        {
            TestDataBuilder.Jobs.AssignedAt(_contractorId, _targetDate, 10, 0, 3.0),
            TestDataBuilder.Jobs.AssignedAt(_contractorId, _targetDate, 11, 0, 3.0)
        };

        // Act
        var result = _service.CalculateAvailability(
            workingHours,
            jobs,
            requiredDurationHours: 1.0).ToList();

        // Assert - Should merge overlapping slots and return gaps
        result.Should().HaveCount(2);
        result[0].Start.Should().Be(new TimeOnly(9, 0));
        result[0].End.Should().Be(new TimeOnly(10, 0));
        result[1].Start.Should().Be(new TimeOnly(14, 0));
        result[1].End.Should().Be(new TimeOnly(17, 0));
    }

    #endregion

    #region Duration Filtering Tests

    [Fact]
    public void CalculateAvailability_FiltersGapsSmallerThanRequired()
    {
        // Arrange - Create 30-minute gap
        var workingHours = new List<WeeklySchedule>
        {
            TestDataBuilder.Schedules.StandardWorkDay(_contractorId, DayOfWeek.Monday)
        };
        var jobs = new List<Job>
        {
            TestDataBuilder.Jobs.AssignedAt(_contractorId, _targetDate, 9, 0, 3.5),
            TestDataBuilder.Jobs.AssignedAt(_contractorId, _targetDate, 13, 0, 4.0)
        };

        // Act - Need 1 hour, but only 30-minute gap available
        var result = _service.CalculateAvailability(
            workingHours,
            jobs,
            requiredDurationHours: 1.0);

        // Assert - Should exclude the small gap
        result.Should().BeEmpty();
    }

    [Fact]
    public void CalculateAvailability_IncludesGapsEqualToRequired()
    {
        // Arrange - Create exactly 2-hour gap
        var workingHours = new List<WeeklySchedule>
        {
            TestDataBuilder.Schedules.StandardWorkDay(_contractorId, DayOfWeek.Monday)
        };
        var jobs = new List<Job>
        {
            TestDataBuilder.Jobs.AssignedAt(_contractorId, _targetDate, 9, 0, 3.0),
            TestDataBuilder.Jobs.AssignedAt(_contractorId, _targetDate, 14, 0, 3.0)
        };

        // Act - Need exactly 2 hours
        var result = _service.CalculateAvailability(
            workingHours,
            jobs,
            requiredDurationHours: 2.0).ToList();

        // Assert - Should include the exact-fit gap
        result.Should().HaveCount(1);
        result[0].DurationHours.Should().Be(2.0);
    }

    #endregion

    #region Split Shift Tests

    [Fact]
    public void CalculateAvailability_WithSplitShiftAndJobs_HandlesCorrectly()
    {
        // Arrange - Split shift with job in afternoon
        var workingHours = TestDataBuilder.Schedules.SplitShift(
            _contractorId,
            DayOfWeek.Monday,
            morningStart: 9,
            morningEnd: 12,
            afternoonStart: 14,
            afternoonEnd: 17);
        var jobs = new List<Job>
        {
            TestDataBuilder.Jobs.AssignedAt(_contractorId, _targetDate, 15, 0, 1.5)
        };

        // Act - Need 0.5 hours (30 minutes)
        var result = _service.CalculateAvailability(
            workingHours,
            jobs,
            requiredDurationHours: 0.5).ToList();

        // Assert - Morning fully available, afternoon has gaps before and after job
        result.Should().HaveCount(3);
        result[0].Start.Should().Be(new TimeOnly(9, 0));
        result[0].End.Should().Be(new TimeOnly(12, 0));
        result[1].Start.Should().Be(new TimeOnly(14, 0));
        result[1].End.Should().Be(new TimeOnly(15, 0));
        result[2].Start.Should().Be(new TimeOnly(16, 30));
        result[2].End.Should().Be(new TimeOnly(17, 0));
    }

    #endregion

    #region Null/Exception Tests

    [Fact]
    public void CalculateAvailability_WithNullWorkingHours_ThrowsException()
    {
        // Act
        var act = () => _service.CalculateAvailability(
            null!,
            new List<Job>(),
            requiredDurationHours: 2.0);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("workingHours");
    }

    [Fact]
    public void CalculateAvailability_WithNullJobs_ThrowsException()
    {
        // Arrange
        var workingHours = new List<WeeklySchedule>
        {
            TestDataBuilder.Schedules.StandardWorkDay(_contractorId, DayOfWeek.Monday)
        };

        // Act
        var act = () => _service.CalculateAvailability(
            workingHours,
            null!,
            requiredDurationHours: 2.0);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("existingJobs");
    }

    #endregion
}
