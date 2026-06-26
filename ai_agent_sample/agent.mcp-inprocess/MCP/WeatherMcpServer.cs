using ModelContextProtocol.Server;
using System.ComponentModel;

namespace mcp.server.MCP
{
    [McpServerToolType]
    public class WeatherMcpServer
    {
        // The attribute tells the MCP framework this method is an executable AI tool
        [McpServerTool(Name = "get_temperature")]

        public async Task<string> GetTemperatureAsync(
            [Description("The name of the city, e.g. New York")] string city)
        {
            // Keep it simple for the presentation demo
            string normalizedCity = city.ToLower().Trim();

            if (normalizedCity == "new york")
                return "The temperature in New York is 12°C.";

            if (normalizedCity == "pune")
                return "The temperature in Pune is 31°C.";

            return $"The temperature in {city} is 22°C.";
        }
    }

}
