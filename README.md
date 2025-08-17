# Sky Tracker MCP
Sky Tracker [MCP](https://de.wikipedia.org/wiki/Model_Context_Protocol) is a .NET-based application designed to interact with flight data from [OpenSky Network API](https://opensky-network.org/data/api). It provides mcp tooling to integrate the api with an mcp client such as [Visual Studio Code](https://code.visualstudio.com/download), [Claude Desktop](https://claude.ai/download) or any other compatible client.

## Getting Started
1. Clone the repository.
2. Configure API keys and settings in `appsettings.json`. You can also leave it empty but with limited access to open sky api.
3. Build and run the solution using Visual Studio or `dotnet` CLI
4. Connect with an MCP client and chat with open sky network. 

## Limitations
Note that getting more than 500 flights leads to wrong results of some clients. Thus it is recommended to as specific questions about an area or aircraft yet. This will be improved in future.  

## Requirements
- .NET 8.0 SDK
- Optional: Free account on [OpenSky Network API](https://opensky-network.org/data/api)

## Example MCP Client Integration

Below is an example JSON configuration for integrating with the Sky Tracker MCP server in an MCP client.

```json
{
	"servers": {
		"sky-tracker-mcp": {
			"url": "http://localhost:5000",
			"type": "http"
		}
	},
	"inputs": []
}
```
