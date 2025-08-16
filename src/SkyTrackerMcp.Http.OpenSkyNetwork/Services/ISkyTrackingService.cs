using SkyTrackerMcp.Http.OpenSkyNetwork.Services.Models;

namespace SkyTrackerMcp.Http.OpenSkyNetwork.Services;

public interface ISkyTrackingService
{
    Task<List<OpenSkyState>> GetActiveFlightsAsync();

    Task<OpenSkyState?> GetFlightByIcaoAsync(string icao24);

    Task<List<OpenSkyState>> GetFlightsByAreaAsync(double south, double north, double east, double west);
}