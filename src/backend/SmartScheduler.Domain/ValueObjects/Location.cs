namespace SmartScheduler.Domain.ValueObjects;

/// <summary>
/// Represents a geographic location with latitude and longitude coordinates.
/// Immutable value object with 6 decimal places precision (~0.1m accuracy).
/// </summary>
public sealed record Location
{
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public string? Address { get; init; }

    // EF Core constructor
    private Location() { }

    public Location(double latitude, double longitude, string? address = null)
    {
        if (latitude < -90 || latitude > 90)
            throw new ArgumentOutOfRangeException(
                nameof(latitude),
                "Latitude must be between -90 and 90 degrees");

        if (longitude < -180 || longitude > 180)
            throw new ArgumentOutOfRangeException(
                nameof(longitude),
                "Longitude must be between -180 and 180 degrees");

        // Round to 6 decimal places for consistency
        Latitude = Math.Round(latitude, 6);
        Longitude = Math.Round(longitude, 6);
        Address = address?.Trim();
    }

    /// <summary>
    /// Calculates straight-line distance to another location in kilometers.
    /// Uses Haversine formula for spherical distance calculation.
    /// </summary>
    public double DistanceToKilometers(Location other)
    {
        const double earthRadiusKm = 6371.0;

        var lat1Rad = Latitude * Math.PI / 180;
        var lat2Rad = other.Latitude * Math.PI / 180;
        var deltaLatRad = (other.Latitude - Latitude) * Math.PI / 180;
        var deltaLonRad = (other.Longitude - Longitude) * Math.PI / 180;

        var a = Math.Sin(deltaLatRad / 2) * Math.Sin(deltaLatRad / 2) +
                Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                Math.Sin(deltaLonRad / 2) * Math.Sin(deltaLonRad / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return earthRadiusKm * c;
    }

    /// <summary>
    /// Calculates straight-line distance to another location in miles.
    /// </summary>
    public double DistanceToMiles(Location other)
    {
        return DistanceToKilometers(other) * 0.621371;
    }
}
