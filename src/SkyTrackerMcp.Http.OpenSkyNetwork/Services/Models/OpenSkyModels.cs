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
    [JsonPropertyName("icao24")]
    public string Icao24 { get; set; } = string.Empty;
    
    [JsonPropertyName("callsign")]
    public string? Callsign { get; set; }
    
    [JsonPropertyName("origin_country")]
    public string? OriginCountry { get; set; }
    
    [JsonPropertyName("time_position")]
    public long? TimePosition { get; set; }
    
    [JsonPropertyName("last_contact")]
    public long LastContact { get; set; }
    
    [JsonPropertyName("longitude")]
    public double? Longitude { get; set; }
    
    [JsonPropertyName("latitude")]
    public double? Latitude { get; set; }
    
    [JsonPropertyName("baro_altitude")]
    public double? BaroAltitude { get; set; }
    
    [JsonPropertyName("on_ground")]
    public bool OnGround { get; set; }
    
    [JsonPropertyName("velocity")]
    public double? Velocity { get; set; }
    
    [JsonPropertyName("true_track")]
    public double? TrueTrack { get; set; }
    
    [JsonPropertyName("vertical_rate")]
    public double? VerticalRate { get; set; }
    
    [JsonPropertyName("geo_altitude")]
    public double? GeoAltitude { get; set; }
    
    [JsonPropertyName("squawk")]
    public string? Squawk { get; set; }
    
    [JsonPropertyName("spi")]
    public bool Spi { get; set; }
    
    [JsonPropertyName("position_source")]
    public int PositionSource { get; set; }
    
    [JsonPropertyName("category")]
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
