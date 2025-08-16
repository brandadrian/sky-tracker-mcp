using SkyTrackerMcp.Http.OpenSkyNetwork.Services.Models;

namespace SkyTrackerMcp.Http.OpenSkyNetwork.Services;

public interface ISkyTrackingService
{
    Task<List<OpenSkyState>> GetActiveFlightsAsync();
}