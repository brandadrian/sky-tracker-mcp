using System.ComponentModel;
using ModelContextProtocol.Server;
using SkyTrackerMcp.Http.OpenSkyNetwork.Services;
using SkyTrackerMcp.Http.OpenSkyNetwork.Services.Models;

namespace SkyTrackerMcp.McpTools;

[McpServerToolType]
public class SkyTrackerTools
{
    private readonly ISkyTrackingService _skyTrackingService;
 
    public SkyTrackerTools(ISkyTrackingService skyTrackingService)
    {
        _skyTrackingService = skyTrackingService;
    }
    
    [McpServerTool, Description("Returns all active flights.")]
    public async Task<List<OpenSkyState>> GetActiveFlightsAsync()
    {
        return await _skyTrackingService.GetActiveFlightsAsync();
    }
    
    [McpServerTool, Description("Returns a flight by ICAO.")]
    public async Task<OpenSkyState?> GetFlightByIcaoAsync(
        [Description("Name of the city to return weather for")] string icao
        )
    {
        return await _skyTrackingService.GetFlightByIcaoAsync(icao);
    }
    
    /// <summary>
    /// Get Flights by area.
    /// Example for switzerland based on opensky api https://opensky-network.org/api/states/all?lamin=45.8389&lomin=5.9962&lamax=47.8229&lomax=10.5226
    /// </summary>
    /// <param name="south"></param>
    /// <param name="noth"></param>
    /// <param name="east"></param>
    /// <param name="west"></param>
    /// <returns></returns>
    [McpServerTool, Description("Returns all flights within the defined area.")]
    public async Task<List<OpenSkyState>> GetFlightsByAreaAsync(
        [Description("Lower bound for the latitude in decimal degrees")] double south,
        [Description("Lower bound for the longitude in decimal degrees")] double noth,
        [Description("Upper bound for the latitude in decimal degrees")] double east,
        [Description("Upper bound for the longitude in decimal degrees")] double west
        )
    {
        return await _skyTrackingService.GetFlightsByAreaAsync(south, noth, east, west);
    }
}