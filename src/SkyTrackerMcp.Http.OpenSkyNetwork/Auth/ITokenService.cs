namespace SkyTrackerMcp.Http.OpenSkyNetwork.Auth;

public interface ITokenService
{
    Task<string> GetAccessTokenAsync();
}