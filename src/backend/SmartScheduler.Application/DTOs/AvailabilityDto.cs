namespace SmartScheduler.Application.DTOs;

/// <summary>
/// DTO representing contractor availability for a specific date.
/// </summary>
public class AvailabilityDto
{
    public Guid ContractorId { get; set; }
    public string ContractorFormattedId { get; set; } = string.Empty;
    public string ContractorName { get; set; } = string.Empty;
    public DateTime TargetDate { get; set; }
    public List<TimeSlotDto> AvailableSlots { get; set; } = new();
}
