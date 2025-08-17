using System.ComponentModel;
using ModelContextProtocol.Server;
using SkyTrackerMcp.Http.OpenSkyNetwork.Services;
using SkyTrackerMcp.Http.OpenSkyNetwork.Services.Models;
using SkyTrackerMcp.McpTools.Exceptions;

namespace SkyTrackerMcp.McpTools;

/// <summary>
/// Provides tools for interacting with the OpenSky Network.
/// </summary>
[McpServerToolType]
public class SkyTrackerTools
{
    private readonly ISkyTrackingService _skyTrackingService;
 
    public SkyTrackerTools(ISkyTrackingService skyTrackingService)
    {
        _skyTrackingService = skyTrackingService;
    }
        
    /// <summary>
    /// Get a flight by ICAO.
    /// </summary>
    /// <param name="icao"></param>
    /// <returns></returns>
    [McpServerTool, Description("Returns a flight by ICAO.")]
    public async Task<OpenSkyState?> GetFlightByIcaoAsync([Description("Name of the city to return weather for")] string icao)
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
        var flights = await _skyTrackingService.GetFlightsByAreaAsync(south, noth, east, west);
        ValidateFlights(flights);
        return flights;
    }
    
    /// <summary>
    /// It seems that the mcp client is not able to handle more than 500 flights at once.
    /// </summary>
    /// <param name="flights"></param>
    /// <exception cref="InvalidOperationException"></exception>
    private void ValidateFlights(List<OpenSkyState> flights)
    {
        var maxFlights = 500;
        
        if (flights.Count > maxFlights)
        {
            throw new TooManyFlightsException(flights.Count, maxFlights);
        }
    }
}