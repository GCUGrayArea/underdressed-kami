using SmartScheduler.Application.DTOs.Contractors;

namespace SmartScheduler.WebApi.Models.Responses;

/// <summary>
/// Response wrapper for contractor data.
/// Maps from ContractorDto to API response format.
/// </summary>
public class ContractorResponse
{
    public Guid Id { get; init; }
    public string FormattedId { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public Guid JobTypeId { get; init; }
    public string JobTypeName { get; init; } = string.Empty;
    public decimal Rating { get; init; }
    public LocationDto BaseLocation { get; init; } = null!;
    public string? Phone { get; init; }
    public string? Email { get; init; }
    public bool IsActive { get; init; }
    public IReadOnlyList<WeeklyScheduleDto> WeeklySchedule { get; init; } =
        Array.Empty<WeeklyScheduleDto>();

    public ContractorResponse() { }

    public ContractorResponse(ContractorDto dto)
    {
        Id = dto.Id;
        FormattedId = dto.FormattedId;
        Name = dto.Name;
        JobTypeId = dto.JobTypeId;
        JobTypeName = dto.JobTypeName;
        Rating = dto.Rating;
        BaseLocation = new LocationDto
        {
            Latitude = dto.BaseLocation.Latitude,
            Longitude = dto.BaseLocation.Longitude,
            Address = dto.BaseLocation.Address
        };
        Phone = dto.Phone;
        Email = dto.Email;
        IsActive = dto.IsActive;
        WeeklySchedule = dto.WeeklySchedule;
    }
}

/// <summary>
/// Location data for API responses.
/// </summary>
public class LocationDto
{
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public string? Address { get; init; }
}
