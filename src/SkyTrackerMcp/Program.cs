using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using SkyTrackerMcp.Http.OpenSkyNetwork.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithHttpTransport()
    .WithToolsFromAssembly();

builder.Services.AddOpenSkyNetwork(builder.Configuration);

var app = builder.Build();

app.MapMcp();

await app.RunAsync();