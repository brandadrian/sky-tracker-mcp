using System.ComponentModel;
using ModelContextProtocol.Server;
using SkyTrackerMcp.Http.OpenSkyNetwork.Services;
using SkyTrackerMcp.Http.OpenSkyNetwork.Services.Models;

namespace SkyTrackerMcp.McpTools;

[McpServerToolType]
public class SkyTrackerTools
{
    private readonly ISkyTrackingService _todosService;
 
    public SkyTrackerTools(ISkyTrackingService todosService)
    {
        _todosService = todosService;
    }
    
    [McpServerTool, Description("Returns all active flights.")]
    public async Task<List<OpenSkyState>> GetActiveFlightsAsync()
    {
        return await _todosService.GetActiveFlightsAsync();
    }
}