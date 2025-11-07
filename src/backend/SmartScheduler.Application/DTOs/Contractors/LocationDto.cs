namespace SmartScheduler.Application.DTOs.Contractors;

/// <summary>
/// DTO for geographic location data.
/// Used in contractor read operations.
/// </summary>
public class LocationDto
{
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public string? Address { get; init; }

    public LocationDto() { }

    public LocationDto(double latitude, double longitude, string? address = null)
    {
        Latitude = latitude;
        Longitude = longitude;
        Address = address;
    }
}
