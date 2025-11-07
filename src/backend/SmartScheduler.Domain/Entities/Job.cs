using SmartScheduler.Domain.ValueObjects;

namespace SmartScheduler.Domain.Entities;

/// <summary>
/// Represents a flooring job request that can be assigned to a contractor.
/// </summary>
public class Job
{
    public Guid Id { get; private set; }
    public string FormattedId { get; private set; } = string.Empty;
    public Guid JobTypeId { get; private set; }
    public string CustomerId { get; private set; } = string.Empty;
    public string CustomerName { get; private set; } = string.Empty;
    public Location Location { get; private set; } = null!;
    public DateTime DesiredDate { get; private set; }
    public TimeOnly? DesiredTime { get; private set; }
    public decimal EstimatedDurationHours { get; private set; }
    public JobStatus Status { get; private set; }
    public Guid? AssignedContractorId { get; private set; }
    public DateTime? ScheduledStartTime { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    // EF Core constructor
    private Job() { }

    public Job(
        string formattedId,
        Guid jobTypeId,
        string customerId,
        string customerName,
        Location location,
        DateTime desiredDate,
        decimal estimatedDurationHours,
        TimeOnly? desiredTime = null)
    {
        if (string.IsNullOrWhiteSpace(formattedId))
            throw new ArgumentException("Formatted ID cannot be empty", nameof(formattedId));

        if (jobTypeId == Guid.Empty)
            throw new ArgumentException("Job type ID cannot be empty", nameof(jobTypeId));

        if (string.IsNullOrWhiteSpace(customerId))
            throw new ArgumentException("Customer ID cannot be empty", nameof(customerId));

        if (string.IsNullOrWhiteSpace(customerName))
            throw new ArgumentException("Customer name cannot be empty", nameof(customerName));

        if (estimatedDurationHours <= 0)
            throw new ArgumentException(
                "Estimated duration must be positive",
                nameof(estimatedDurationHours));

        Id = Guid.NewGuid();
        FormattedId = formattedId;
        JobTypeId = jobTypeId;
        CustomerId = customerId.Trim();
        CustomerName = customerName.Trim();
        Location = location;
        DesiredDate = desiredDate.Date; // Normalize to date only
        DesiredTime = desiredTime;
        EstimatedDurationHours = Math.Round(estimatedDurationHours, 2);
        Status = JobStatus.Unassigned;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(
        Location location,
        DateTime desiredDate,
        decimal estimatedDurationHours,
        TimeOnly? desiredTime = null)
    {
        if (estimatedDurationHours <= 0)
            throw new ArgumentException(
                "Estimated duration must be positive",
                nameof(estimatedDurationHours));

        Location = location;
        DesiredDate = desiredDate.Date;
        DesiredTime = desiredTime;
        EstimatedDurationHours = Math.Round(estimatedDurationHours, 2);
    }

    public void AssignToContractor(Guid contractorId, DateTime scheduledStartTime)
    {
        if (contractorId == Guid.Empty)
            throw new ArgumentException("Contractor ID cannot be empty", nameof(contractorId));

        if (Status != JobStatus.Unassigned)
            throw new InvalidOperationException(
                $"Cannot assign job in {Status} status");

        AssignedContractorId = contractorId;
        ScheduledStartTime = scheduledStartTime;
        Status = JobStatus.Assigned;
    }

    public void Unassign()
    {
        if (Status != JobStatus.Assigned)
            throw new InvalidOperationException(
                $"Cannot unassign job in {Status} status");

        AssignedContractorId = null;
        ScheduledStartTime = null;
        Status = JobStatus.Unassigned;
    }

    public void MarkInProgress()
    {
        if (Status != JobStatus.Assigned)
            throw new InvalidOperationException(
                $"Cannot start job in {Status} status");

        Status = JobStatus.InProgress;
    }

    public void MarkCompleted()
    {
        if (Status != JobStatus.InProgress)
            throw new InvalidOperationException(
                $"Cannot complete job in {Status} status");

        Status = JobStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == JobStatus.Completed)
            throw new InvalidOperationException("Cannot cancel completed job");

        Status = JobStatus.Cancelled;
    }
}

public enum JobStatus
{
    Unassigned = 0,
    Assigned = 1,
    InProgress = 2,
    Completed = 3,
    Cancelled = 4
}
