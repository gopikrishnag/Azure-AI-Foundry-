namespace UserManagement.Api
{
    //Note: To run the MCP Inspector for this server, use the following command:
    //npx @modelcontextprotocol/inspector
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddMcpServer().WithHttpTransport().WithTools<Tools.WeatherAlertsTool>();
            builder.Services.AddHttpClient("WeatherApi", client =>
            {
                client.BaseAddress = new Uri("https://api.weather.gov/");
                client.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("weather-tool", "1.0"));
            });
            var app = builder.Build();
            app.MapMcp();
            app.Run();
        }
    }
}
