namespace SkyTrackerMcp.Http.OpenSkyNetwork.Configuration;

public class OpenSkyNetworkOptions
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string AuthUrl { get; set; } = "https://auth.opensky-network.org/auth/realms/opensky-network/protocol/openid-connect/token";
    public string BaseUrl { get; set; } = "https://opensky-network.org/api";
}