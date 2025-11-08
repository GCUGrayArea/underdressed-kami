using System.Text.Json.Serialization;

namespace SmartScheduler.Infrastructure.ExternalServices.Models;

/// <summary>
/// Response model for OpenRouteService API directions endpoint.
/// Parses GeoJSON response to extract distance and duration from route summary.
/// </summary>
public sealed class RouteResponse
{
    /// <summary>
    /// Array of route features (typically contains one route).
    /// </summary>
    [JsonPropertyName("features")]
    public RouteFeature[] Features { get; set; } = Array.Empty<RouteFeature>();

    /// <summary>
    /// Extracts distance in meters from the first route feature.
    /// </summary>
    public double GetDistanceMeters()
    {
        return Features.FirstOrDefault()?.Properties?.Summary?.Distance ?? 0;
    }

    /// <summary>
    /// Extracts duration in seconds from the first route feature.
    /// </summary>
    public double GetDurationSeconds()
    {
        return Features.FirstOrDefault()?.Properties?.Summary?.Duration ?? 0;
    }
}

/// <summary>
/// GeoJSON feature containing route geometry and properties.
/// </summary>
public sealed class RouteFeature
{
    [JsonPropertyName("properties")]
    public RouteProperties? Properties { get; set; }
}

/// <summary>
/// Route properties containing summary information.
/// </summary>
public sealed class RouteProperties
{
    [JsonPropertyName("summary")]
    public RouteSummary? Summary { get; set; }
}

/// <summary>
/// Route summary with distance and duration.
/// </summary>
public sealed class RouteSummary
{
    /// <summary>
    /// Total distance in meters.
    /// </summary>
    [JsonPropertyName("distance")]
    public double Distance { get; set; }

    /// <summary>
    /// Total duration in seconds.
    /// </summary>
    [JsonPropertyName("duration")]
    public double Duration { get; set; }
}
