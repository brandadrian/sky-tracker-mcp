using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SkyTrackerMcp.Http.OpenSkyNetwork.Auth.Models;
using SkyTrackerMcp.Http.OpenSkyNetwork.Configuration;

namespace SkyTrackerMcp.Http.OpenSkyNetwork.Auth;

internal class TokenService : ITokenService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TokenService> _logger;
    private readonly OpenSkyNetworkOptions _openSkyNetworkOptions;
    private string? _cachedToken;
    private DateTime _tokenExpiry = DateTime.MinValue;
    private readonly SemaphoreSlim _tokenSemaphore = new(1, 1);

    public TokenService(
        HttpClient httpClient, 
        ILogger<TokenService> logger,
        IOptions<OpenSkyNetworkOptions> openSkyNetworkOptions)
    {
        _httpClient = httpClient;
        _logger = logger;
        _openSkyNetworkOptions = openSkyNetworkOptions.Value;
    }

    public async Task<string> GetAccessTokenAsync()
    {
        await _tokenSemaphore.WaitAsync();
        try
        {
            // Check if we have a valid cached token
            if (!string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _tokenExpiry)
            {
                _logger.LogDebug("Using cached access token");
                return _cachedToken;
            }

            _logger.LogInformation("Requesting new access token from OpenSky Network");

            var tokenRequest = new List<KeyValuePair<string, string>>
            {
                new("grant_type", "client_credentials"),
                new("client_id", _openSkyNetworkOptions.ClientId),
                new("client_secret", _openSkyNetworkOptions.ClientSecret)
            };

            var requestContent = new FormUrlEncodedContent(tokenRequest);
            
            var response = await _httpClient.PostAsync(_openSkyNetworkOptions.AuthUrl, requestContent);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent);

            if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
            {
                throw new InvalidOperationException("Invalid token response from OpenSky Network");
            }

            _cachedToken = tokenResponse.AccessToken;
            // Set expiry with a 5-minute buffer to avoid using expired tokens
            _tokenExpiry = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn - 300);

            _logger.LogInformation("Successfully obtained access token, expires at {Expiry}", _tokenExpiry);

            return _cachedToken;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error while requesting access token");
            throw new InvalidOperationException("Failed to obtain access token from OpenSky Network", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON parsing error while processing token response");
            throw new InvalidOperationException("Failed to parse token response from OpenSky Network", ex);
        }
        finally
        {
            _tokenSemaphore.Release();
        }
    }
}
