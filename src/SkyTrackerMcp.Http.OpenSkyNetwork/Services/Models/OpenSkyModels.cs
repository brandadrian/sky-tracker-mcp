using System.Text.Json.Serialization;

namespace SkyTrackerMcp.Http.OpenSkyNetwork.Services.Models;

/// <summary>
/// OpenSky Network API response model for states endpoint
/// </summary>
public class OpenSkyStatesResponse
{
    [JsonPropertyName("time")]
    public long Time { get; set; }

    [JsonPropertyName("states")]
    public object[][]? States { get; set; }
}

/// <summary>
/// Simple model representing a flight state from OpenSky Network
/// Uses object array for direct JSON deserialization
/// </summary>
public class OpenSkyState
{
    public string Icao24 { get; set; } = string.Empty;
    public string? Callsign { get; set; }
    public string? OriginCountry { get; set; }
    public long? TimePosition { get; set; }
    public long LastContact { get; set; }
    public double? Longitude { get; set; }
    public double? Latitude { get; set; }
    public double? BaroAltitude { get; set; }
    public bool OnGround { get; set; }
    public double? Velocity { get; set; }
    public double? TrueTrack { get; set; }
    public double? VerticalRate { get; set; }
    public double? GeoAltitude { get; set; }
    public string? Squawk { get; set; }
    public bool Spi { get; set; }
    public int PositionSource { get; set; }
    public int? Category { get; set; }

    /// <summary>
    /// Creates an OpenSkyState from an object array
    /// </summary>
    public static OpenSkyState? FromArray(object[] state)
    {
        if (state.Length < 17) return null;
        
        try
        {
            return new OpenSkyState
            {
                Icao24 = state[0]?.ToString() ?? string.Empty,
                Callsign = state[1]?.ToString()?.Trim(),
                OriginCountry = state[2]?.ToString(),
                TimePosition = ConvertToLong(state[3]),
                LastContact = ConvertToLong(state[4]) ?? 0,
                Longitude = ConvertToDouble(state[5]),
                Latitude = ConvertToDouble(state[6]),
                BaroAltitude = ConvertToDouble(state[7]),
                OnGround = ConvertToBool(state[8]),
                Velocity = ConvertToDouble(state[9]),
                TrueTrack = ConvertToDouble(state[10]),
                VerticalRate = ConvertToDouble(state[11]),
                GeoAltitude = state.Length > 13 ? ConvertToDouble(state[13]) : null,
                Squawk = state.Length > 14 ? state[14]?.ToString() : null,
                Spi = state.Length > 15 && ConvertToBool(state[15]),
                PositionSource = state.Length > 16 ? ConvertToInt(state[16]) : 0,
                Category = state.Length > 17 ? ConvertToInt(state[17]) : null
            };
        }
        catch
        {
            return null;
        }
    }

    private static long? ConvertToLong(object? value)
    {
        if (value == null) return null;
        if (long.TryParse(value.ToString(), out var result)) return result;
        return null;
    }

    private static double? ConvertToDouble(object? value)
    {
        if (value == null) return null;
        if (double.TryParse(value.ToString(), out var result)) return result;
        return null;
    }

    private static bool ConvertToBool(object? value)
    {
        if (value == null) return false;
        if (value.ToString() == "true") return true;
        return false;
    }

    private static int ConvertToInt(object? value)
    {
        if (value == null) return 0;
        if (int.TryParse(value.ToString(), out var result)) return result;
        return 0;
    }
}
