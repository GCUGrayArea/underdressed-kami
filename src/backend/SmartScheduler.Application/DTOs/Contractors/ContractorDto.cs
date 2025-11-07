namespace SmartScheduler.Application.DTOs.Contractors;

/// <summary>
/// Complete DTO for contractor details.
/// Includes all information including location and weekly schedule.
/// Used for single contractor retrieval.
/// </summary>
public class ContractorDto
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
    public IReadOnlyList<WeeklyScheduleDto> WeeklySchedule { get; init; } = Array.Empty<WeeklyScheduleDto>();

    public ContractorDto() { }

    public ContractorDto(
        Guid id,
        string formattedId,
        string name,
        Guid jobTypeId,
        string jobTypeName,
        decimal rating,
        LocationDto baseLocation,
        bool isActive,
        IReadOnlyList<WeeklyScheduleDto> weeklySchedule,
        string? phone = null,
        string? email = null)
    {
        Id = id;
        FormattedId = formattedId;
        Name = name;
        JobTypeId = jobTypeId;
        JobTypeName = jobTypeName;
        Rating = rating;
        BaseLocation = baseLocation;
        IsActive = isActive;
        WeeklySchedule = weeklySchedule;
        Phone = phone;
        Email = email;
    }
}
