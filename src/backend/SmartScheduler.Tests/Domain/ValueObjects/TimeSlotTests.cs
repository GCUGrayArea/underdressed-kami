using FluentAssertions;
using SmartScheduler.Domain.ValueObjects;

namespace SmartScheduler.Tests.Domain.ValueObjects;

/// <summary>
/// Unit tests for TimeSlot value object validating immutability,
/// validation, and helper methods (Overlaps, Contains, CanAccommodate).
/// </summary>
public class TimeSlotTests
{
    [Fact]
    public void Constructor_WithValidTimes_CreatesTimeSlot()
    {
        // Arrange
        var start = new TimeOnly(9, 0);
        var end = new TimeOnly(17, 0);

        // Act
        var slot = new TimeSlot(start, end);

        // Assert
        slot.Start.Should().Be(start);
        slot.End.Should().Be(end);
    }

    [Fact]
    public void Constructor_WithEndBeforeStart_ThrowsArgumentException()
    {
        // Arrange
        var start = new TimeOnly(17, 0);
        var end = new TimeOnly(9, 0);

        // Act
        var act = () => new TimeSlot(start, end);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("End time must be after start time*");
    }

    [Fact]
    public void Constructor_WithEqualStartAndEnd_ThrowsArgumentException()
    {
        // Arrange
        var time = new TimeOnly(9, 0);

        // Act
        var act = () => new TimeSlot(time, time);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("End time must be after start time*");
    }

    [Fact]
    public void DurationHours_CalculatesCorrectDuration()
    {
        // Arrange
        var slot = new TimeSlot(new TimeOnly(9, 0), new TimeOnly(13, 30));

        // Act
        var duration = slot.DurationHours;

        // Assert
        duration.Should().Be(4.5);
    }

    [Fact]
    public void Overlaps_WithOverlappingSlot_ReturnsTrue()
    {
        // Arrange
        var slot1 = new TimeSlot(new TimeOnly(9, 0), new TimeOnly(12, 0));
        var slot2 = new TimeSlot(new TimeOnly(11, 0), new TimeOnly(14, 0));

        // Act
        var result = slot1.Overlaps(slot2);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Overlaps_WithNonOverlappingSlot_ReturnsFalse()
    {
        // Arrange
        var slot1 = new TimeSlot(new TimeOnly(9, 0), new TimeOnly(12, 0));
        var slot2 = new TimeSlot(new TimeOnly(14, 0), new TimeOnly(17, 0));

        // Act
        var result = slot1.Overlaps(slot2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Overlaps_WithAdjacentSlot_ReturnsFalse()
    {
        // Arrange - back-to-back slots
        var slot1 = new TimeSlot(new TimeOnly(9, 0), new TimeOnly(12, 0));
        var slot2 = new TimeSlot(new TimeOnly(12, 0), new TimeOnly(15, 0));

        // Act
        var result = slot1.Overlaps(slot2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Overlaps_WithContainedSlot_ReturnsTrue()
    {
        // Arrange - slot2 completely inside slot1
        var slot1 = new TimeSlot(new TimeOnly(9, 0), new TimeOnly(17, 0));
        var slot2 = new TimeSlot(new TimeOnly(11, 0), new TimeOnly(14, 0));

        // Act
        var result = slot1.Overlaps(slot2);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Overlaps_WithNullSlot_ThrowsArgumentNullException()
    {
        // Arrange
        var slot = new TimeSlot(new TimeOnly(9, 0), new TimeOnly(12, 0));

        // Act
        var act = () => slot.Overlaps(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Contains_WithTimeInsideSlot_ReturnsTrue()
    {
        // Arrange
        var slot = new TimeSlot(new TimeOnly(9, 0), new TimeOnly(17, 0));
        var time = new TimeOnly(12, 30);

        // Act
        var result = slot.Contains(time);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Contains_WithTimeAtStart_ReturnsTrue()
    {
        // Arrange
        var slot = new TimeSlot(new TimeOnly(9, 0), new TimeOnly(17, 0));
        var time = new TimeOnly(9, 0);

        // Act
        var result = slot.Contains(time);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Contains_WithTimeAtEnd_ReturnsFalse()
    {
        // Arrange - end is exclusive
        var slot = new TimeSlot(new TimeOnly(9, 0), new TimeOnly(17, 0));
        var time = new TimeOnly(17, 0);

        // Act
        var result = slot.Contains(time);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Contains_WithTimeBeforeSlot_ReturnsFalse()
    {
        // Arrange
        var slot = new TimeSlot(new TimeOnly(9, 0), new TimeOnly(17, 0));
        var time = new TimeOnly(8, 0);

        // Act
        var result = slot.Contains(time);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Contains_WithTimeAfterSlot_ReturnsFalse()
    {
        // Arrange
        var slot = new TimeSlot(new TimeOnly(9, 0), new TimeOnly(17, 0));
        var time = new TimeOnly(18, 0);

        // Act
        var result = slot.Contains(time);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CanAccommodate_WithSufficientDuration_ReturnsTrue()
    {
        // Arrange - 4 hour slot, 3 hour job
        var slot = new TimeSlot(new TimeOnly(9, 0), new TimeOnly(13, 0));

        // Act
        var result = slot.CanAccommodate(3.0);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanAccommodate_WithExactDuration_ReturnsTrue()
    {
        // Arrange - 4 hour slot, 4 hour job
        var slot = new TimeSlot(new TimeOnly(9, 0), new TimeOnly(13, 0));

        // Act
        var result = slot.CanAccommodate(4.0);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CanAccommodate_WithInsufficientDuration_ReturnsFalse()
    {
        // Arrange - 2 hour slot, 3 hour job
        var slot = new TimeSlot(new TimeOnly(9, 0), new TimeOnly(11, 0));

        // Act
        var result = slot.CanAccommodate(3.0);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_WithSameStartAndEnd_ReturnsTrue()
    {
        // Arrange
        var slot1 = new TimeSlot(new TimeOnly(9, 0), new TimeOnly(17, 0));
        var slot2 = new TimeSlot(new TimeOnly(9, 0), new TimeOnly(17, 0));

        // Act & Assert
        slot1.Equals(slot2).Should().BeTrue();
        (slot1 == slot2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentTimes_ReturnsFalse()
    {
        // Arrange
        var slot1 = new TimeSlot(new TimeOnly(9, 0), new TimeOnly(17, 0));
        var slot2 = new TimeSlot(new TimeOnly(10, 0), new TimeOnly(17, 0));

        // Act & Assert
        slot1.Equals(slot2).Should().BeFalse();
        (slot1 != slot2).Should().BeTrue();
    }

    [Fact]
    public void GetHashCode_WithSameValues_ReturnsSameHash()
    {
        // Arrange
        var slot1 = new TimeSlot(new TimeOnly(9, 0), new TimeOnly(17, 0));
        var slot2 = new TimeSlot(new TimeOnly(9, 0), new TimeOnly(17, 0));

        // Act & Assert
        slot1.GetHashCode().Should().Be(slot2.GetHashCode());
    }

    [Fact]
    public void ToString_FormatsCorrectly()
    {
        // Arrange
        var slot = new TimeSlot(new TimeOnly(9, 30), new TimeOnly(17, 45));

        // Act
        var result = slot.ToString();

        // Assert
        result.Should().Be("09:30 - 17:45");
    }
}
