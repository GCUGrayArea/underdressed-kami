namespace SmartScheduler.Application.DTOs;

/// <summary>
/// Data transfer object for location information.
/// </summary>
public class LocationDto
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? Address { get; set; }
}
