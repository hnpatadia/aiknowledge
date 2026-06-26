using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Tools
{
    [McpServerToolType]
    public class WeatherMcpServer
    {
        private readonly ILogger<WeatherMcpServer> _logger;

        public WeatherMcpServer(ILogger<WeatherMcpServer> logger)
        {
            _logger = logger;
        }

        // The attribute tells the MCP framework this method is an executable AI tool
        [McpServerTool(Name = "get_temperature")]

        public Task<string> GetTemperatureAsync(
            [Description("The name of the city, e.g. New York")] string city)
        {
            _logger.LogInformation("GetTemperatureAsync called with city: {City}", city);

            // Keep it simple for the presentation demo
            string normalizedCity = city?.ToLower().Trim();

            if (normalizedCity == "new york")
            {
                var res = "The temperature in New York is 12°C.";
                _logger.LogInformation("Matched New York, returning: {Result}", res);
                return Task.FromResult(res);
            }

            if (normalizedCity == "pune")
            {
                var res = "The temperature in Pune is 31°C.";
                _logger.LogInformation("Matched Pune, returning: {Result}", res);
                return Task.FromResult(res);
            }

            var defaultRes = $"The temperature in {city} is 22°C.";
            _logger.LogInformation("Default case, returning: {Result}", defaultRes);
            return Task.FromResult(defaultRes);
        }
    }

}
