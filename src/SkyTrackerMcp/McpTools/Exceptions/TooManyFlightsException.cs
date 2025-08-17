namespace SkyTrackerMcp.McpTools.Exceptions;

public class TooManyFlightsException : Exception
{
    public int FlightCount { get; }
    public int MaxAllowedFlights { get; }

    public TooManyFlightsException(int flightCount, int maxAllowedFlights)
        : base($"Query returned {flightCount} flights, which exceeds the maximum allowed limit of {maxAllowedFlights} flights for MCP client processing.")
    {
        FlightCount = flightCount;
        MaxAllowedFlights = maxAllowedFlights;
    }
}