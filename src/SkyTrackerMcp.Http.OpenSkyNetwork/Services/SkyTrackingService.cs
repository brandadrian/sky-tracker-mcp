using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SkyTrackerMcp.Http.OpenSkyNetwork.Auth;
using SkyTrackerMcp.Http.OpenSkyNetwork.Configuration;
using SkyTrackerMcp.Http.OpenSkyNetwork.Services.Models;

namespace SkyTrackerMcp.Http.OpenSkyNetwork.Services;

internal class SkyTrackingService : ISkyTrackingService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SkyTrackingService> _logger;
    private readonly ITokenService? _tokenService;
    private readonly OpenSkyNetworkOptions _openSkyNetworkOptions;
    private readonly string _baseUrl;

    public SkyTrackingService(
        HttpClient httpClient, 
        ILogger<SkyTrackingService> logger,
        ITokenService tokenService,
        IOptions<OpenSkyNetworkOptions> openSkyNetworkOptions)
    {
        _httpClient = httpClient;
        _logger = logger;
        _tokenService = tokenService;
        _openSkyNetworkOptions = openSkyNetworkOptions.Value;
        _baseUrl = _openSkyNetworkOptions.BaseUrl.TrimEnd('/');
    }

    public async Task<List<OpenSkyState>> GetActiveFlightsAsync()
    {
        try
        {
            _logger.LogInformation("Fetching active flights from OpenSky Network API");
            
            await SetAuthorizationHeaderAsync();
            
            var response = await _httpClient.GetAsync($"{_baseUrl}/states/all");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var openSkyResponse = JsonSerializer.Deserialize<OpenSkyStatesResponse>(content);
            
            if (openSkyResponse?.States == null)
            {
                _logger.LogWarning("No states found in OpenSky API response");
                return [];
            }

            var flights = new List<OpenSkyState>();
            foreach (var stateArray in openSkyResponse.States)
            {
                var openSkyState = OpenSkyState.FromArray(stateArray);
                if (openSkyState != null)
                {
                    flights.Add(openSkyState);
                }
            }

            _logger.LogInformation("Successfully parsed {Count} flights from OpenSky API", flights.Count);

            return flights;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error while fetching active flights");
            throw new InvalidOperationException("Failed to fetch flights from OpenSky Network", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON parsing error while processing OpenSky response");
            throw new InvalidOperationException("Failed to parse OpenSky Network response", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while fetching active flights");
            throw;
        }
    }
    
    public async Task<OpenSkyState?> GetFlightByIcaoAsync(string icao24)
    {
        try
        {
            _logger.LogInformation("Fetching flight with ICAO: {Icao24} from OpenSky Network API", icao24);
            
            await SetAuthorizationHeaderAsync();
            
            var response = await _httpClient.GetAsync($"{_baseUrl}/states/all?icao24={icao24}");
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            var openSkyResponse = JsonSerializer.Deserialize<OpenSkyStatesResponse>(content);
            
            if (openSkyResponse?.States == null || !openSkyResponse.States.Any())
            {
                _logger.LogWarning("No states found for ICAO: {Icao24}", icao24);
                return null;
            }

            var stateArray = openSkyResponse.States.First();
            var flight = OpenSkyState.FromArray(stateArray);
            
            if (flight != null)
            {
                _logger.LogInformation("Found flight with ICAO: {Icao24}", icao24);
            }
            else
            {
                _logger.LogWarning("No flight found with ICAO: {Icao24}", icao24);
            }
            
            return flight;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error while fetching flight with ICAO: {Icao24}", icao24);
            throw new InvalidOperationException($"Failed to fetch flight {icao24} from OpenSky Network", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON parsing error while processing OpenSky response for ICAO: {Icao24}", icao24);
            throw new InvalidOperationException($"Failed to parse OpenSky Network response for flight {icao24}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while fetching flight with ICAO: {Icao24}", icao24);
            throw;
        }
    }

    public async Task<List<OpenSkyState>> GetFlightsByAreaAsync(double south, double north, double east, double west)
    {
        try
        {
            _logger.LogInformation(
                "Fetching flights in area from OpenSky Network API: North={North}, South={South}, East={East}, West={West}",
                north, south, east, west);
            
            await SetAuthorizationHeaderAsync();
            
            var url = $"{_baseUrl}/states/all?lamin={south}&lamax={north}&lomin={west}&lomax={east}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            var openSkyResponse = JsonSerializer.Deserialize<OpenSkyStatesResponse>(content);
            
            if (openSkyResponse?.States == null)
            {
                _logger.LogWarning("No states found in area response");
                return [];
            }

            var flights = new List<OpenSkyState>();
            foreach (var stateArray in openSkyResponse.States)
            {
                var openSkyState = OpenSkyState.FromArray(stateArray);
                if (openSkyState != null)
                {
                    flights.Add(openSkyState);
                }
            }
            
            _logger.LogInformation("Successfully parsed {Count} flights in area from OpenSky API", flights.Count);
            
            return flights;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error while fetching flights by area");
            throw new InvalidOperationException("Failed to fetch flights by area from OpenSky Network", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON parsing error while processing OpenSky area response");
            throw new InvalidOperationException("Failed to parse OpenSky Network area response", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while fetching flights by area");
            throw;
        }
    }

    private async Task SetAuthorizationHeaderAsync()
    {
        if (_tokenService != null)
        {
            try
            {
                var token = await _tokenService.GetAccessTokenAsync();
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                _logger.LogDebug("Authorization header set with bearer token");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to set authorization header, continuing with unauthenticated request");
                // Continue without authentication - some endpoints might still work
            }
        }
    }
}
