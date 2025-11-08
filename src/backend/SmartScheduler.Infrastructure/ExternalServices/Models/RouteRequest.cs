namespace SmartScheduler.Infrastructure.ExternalServices.Models;

/// <summary>
/// Request model for OpenRouteService API directions endpoint.
/// API expects coordinates in [longitude, latitude] order (GeoJSON standard).
/// </summary>
public sealed class RouteRequest
{
    /// <summary>
    /// Array of coordinate pairs: [[lon1, lat1], [lon2, lat2]].
    /// Note: Longitude first, then latitude (GeoJSON standard).
    /// </summary>
    public double[][] Coordinates { get; set; } = Array.Empty<double[]>();

    /// <summary>
    /// Creates a route request for two locations.
    /// </summary>
    public static RouteRequest Create(double originLat, double originLon, double destLat, double destLon)
    {
        return new RouteRequest
        {
            Coordinates = new[]
            {
                new[] { originLon, originLat },  // Origin: [longitude, latitude]
                new[] { destLon, destLat }       // Destination: [longitude, latitude]
            }
        };
    }
}
