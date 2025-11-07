using SmartScheduler.Domain.Entities;

namespace SmartScheduler.Application.DTOs;

/// <summary>
/// Data Transfer Object for Job entity.
/// Used for read operations (queries).
/// </summary>
public class JobDto
{
    public Guid Id { get; set; }
    public string FormattedId { get; set; } = string.Empty;
    public Guid JobTypeId { get; set; }
    public string JobTypeName { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime DesiredDate { get; set; }
    public TimeOnly? DesiredTime { get; set; }
    public decimal EstimatedDurationHours { get; set; }
    public JobStatus Status { get; set; }
    public Guid? AssignedContractorId { get; set; }
    public string? AssignedContractorName { get; set; }
    public string? AssignedContractorFormattedId { get; set; }
    public DateTime? ScheduledStartTime { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
