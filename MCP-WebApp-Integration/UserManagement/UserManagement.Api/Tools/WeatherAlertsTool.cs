using System.ComponentModel;
using System.Text.Json;
using System.Threading.Tasks;
using ModelContextProtocol;
using ModelContextProtocol.Server;

namespace UserManagement.Api.Tools
{
    public class WeatherAlertsTool(IHttpClientFactory httpClientFactory)
    {
        [McpServerTool, Description("Check the health status of the Weather Alerts Tool")]
        public string HealthCheck()
        {
            return "Weather Alerts Tool is operational.";
        }
        [McpServerTool, Description("This tool returns alerts from the https://api.weather.gov API based on the state code.")]

        public async Task<List<Model.Entities.WeatherAlert>> GetWeatherAlerts([Description("2 chars state code for example, NY")] string stateCode)
        {
            var client = httpClientFactory.CreateClient("WeatherApi");
            using var response = await client.GetStreamAsync($"/alerts?area={stateCode}&limit=10");
            using var doc = await JsonDocument.ParseAsync(response) ?? throw new McpException("No JSON returned from the alerts for the state.");
            var features = doc.RootElement.GetProperty("features").EnumerateArray();
            if (!features.Any())
            {
                return [];
            }

            var alerts = features.Select(feature => new Model.Entities.WeatherAlert
            {
                Event = feature.GetProperty("properties").GetProperty("event").GetString() ?? string.Empty,
                AreaDesc = feature.GetProperty("properties").GetProperty("areaDesc").GetString() ?? string.Empty,
                Severity = feature.GetProperty("properties").GetProperty("severity").GetString() ?? string.Empty,
                Description = feature.GetProperty("properties").GetProperty("description").GetString() ?? string.Empty,
            }).ToList();


            return alerts;
        }
    }
}
