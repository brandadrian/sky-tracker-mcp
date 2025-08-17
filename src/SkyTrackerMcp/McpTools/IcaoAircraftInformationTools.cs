using System.ComponentModel;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

namespace SkyTrackerMcp.McpTools;

/// <summary>
/// Provides tools for fetching aircraft information by ICAO code from https://www.flightdb.net.
/// </summary>
[McpServerToolType]
public class IcaoAircraftInformationTools
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<IcaoAircraftInformationTools> _logger;
    private const string BaseUrl = "https://www.flightdb.net";

    public IcaoAircraftInformationTools(HttpClient httpClient, ILogger<IcaoAircraftInformationTools> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    [McpServerTool, Description("Get aircraft information from FlightDB by ICAO code")]
    public async Task<string> GetAircraftInfoAsync([Description("ICAO code (e.g., 4406f0)")] string icao)
    {
        try
        {
            _logger.LogInformation("Fetching aircraft info for ICAO: {Icao} from FlightDB", icao);

            var url = $"{BaseUrl}/aircraft.php?modes={icao}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("Successfully fetched aircraft info for ICAO: {Icao}", icao);

            return content;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error while fetching aircraft info for ICAO: {Icao}", icao);
            return $"Error fetching aircraft info: {ex.Message}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while fetching aircraft info for ICAO: {Icao}", icao);
            return $"Unexpected error: {ex.Message}";
        }
    }

    [McpServerTool, Description("Get aircraft information for multiple ICAO codes from FlightDB")]
    public async Task<Dictionary<string, string>> GetMultipleAircraftInfoAsync(
        [Description("List of ICAO codes (e.g., ['4406f0', '440123', '440456'])")] List<string> icaoCodes)
    {
        var results = new Dictionary<string, string>();
        
        _logger.LogInformation("Fetching aircraft info for {Count} ICAO codes from FlightDB", icaoCodes.Count);

        foreach (var icao in icaoCodes)
        {
            try
            {
                var aircraftInfo = await GetAircraftInfoAsync(icao);
                results[icao] = aircraftInfo;
                
                // Add small delay to be respectful to the API
                await Task.Delay(200);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching aircraft info for ICAO: {Icao}", icao);
                results[icao] = $"Error: {ex.Message}";
            }
        }

        _logger.LogInformation("Successfully processed {Count} aircraft lookups", results.Count);
        return results;
    }
}