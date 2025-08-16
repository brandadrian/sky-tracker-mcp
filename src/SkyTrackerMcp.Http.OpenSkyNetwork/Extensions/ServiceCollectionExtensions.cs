using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SkyTrackerMcp.Http.OpenSkyNetwork.Auth;
using SkyTrackerMcp.Http.OpenSkyNetwork.Configuration;
using SkyTrackerMcp.Http.OpenSkyNetwork.Services;

namespace SkyTrackerMcp.Http.OpenSkyNetwork.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOpenSkyNetwork(
        this IServiceCollection services,
        IConfiguration configuration,
        string configurationSection = "OpenSkyNetwork")
    {
        services.Configure<OpenSkyNetworkOptions>(configuration.GetSection(configurationSection));
        
        services.AddHttpClient<TokenService>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddSingleton<ITokenService, TokenService>();

        services.AddHttpClient<SkyTrackingService>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(30);
        });
        
        services.AddTransient<ISkyTrackingService, SkyTrackingService>();
        
        return services;
    }
}
