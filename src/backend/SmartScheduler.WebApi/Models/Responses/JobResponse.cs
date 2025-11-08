using SmartScheduler.Application.DTOs;
using SmartScheduler.Domain.Entities;

namespace SmartScheduler.WebApi.Models.Responses;

/// <summary>
/// Response wrapper for job data.
/// Maps from JobDto to API response format.
/// </summary>
public class JobResponse
{
    public Guid Id { get; init; }
    public string FormattedId { get; init; } = string.Empty;
    public Guid JobTypeId { get; init; }
    public string JobTypeName { get; init; } = string.Empty;
    public string CustomerId { get; init; } = string.Empty;
    public string CustomerName { get; init; } = string.Empty;
    public LocationDto Location { get; init; } = null!;
    public DateTime DesiredDate { get; init; }
    public TimeOnly? DesiredTime { get; init; }
    public decimal EstimatedDurationHours { get; init; }
    public JobStatus Status { get; init; }
    public Guid? AssignedContractorId { get; init; }
    public string? AssignedContractorName { get; init; }
    public string? AssignedContractorFormattedId { get; init; }
    public DateTime? ScheduledStartTime { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? CompletedAt { get; init; }

    public JobResponse() { }

    public JobResponse(JobDto dto)
    {
        Id = dto.Id;
        FormattedId = dto.FormattedId;
        JobTypeId = dto.JobTypeId;
        JobTypeName = dto.JobTypeName;
        CustomerId = dto.CustomerId;
        CustomerName = dto.CustomerName;
        Location = new LocationDto
        {
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            Address = null
        };
        DesiredDate = dto.DesiredDate;
        DesiredTime = dto.DesiredTime;
        EstimatedDurationHours = dto.EstimatedDurationHours;
        Status = dto.Status;
        AssignedContractorId = dto.AssignedContractorId;
        AssignedContractorName = dto.AssignedContractorName;
        AssignedContractorFormattedId = dto.AssignedContractorFormattedId;
        ScheduledStartTime = dto.ScheduledStartTime;
        CreatedAt = dto.CreatedAt;
        CompletedAt = dto.CompletedAt;
    }
}
