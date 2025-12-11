using Microsoft.Extensions.Azure;
using UserManagementService;

namespace UserManagement.Api
{
    //Note: To run the MCP Inspector for this server, use the following command:
    //npx @modelcontextprotocol/inspector
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSingleton<UserManagementServices.UserService>();
            builder.Services.AddSingleton<DocumentService>();

            builder.Services.AddMcpServer().WithHttpTransport().WithTools<Tools.AzureSearchTools>();
            builder.Services.AddMcpServer().WithHttpTransport().WithTools<Tools.WeatherAlertsTool>();
            builder.Services.AddMcpServer().WithHttpTransport().WithTools<Tools.UserManagementTools>();
           

            

            builder.Services.AddHttpClient("WeatherApi", client =>
            {
                client.BaseAddress = new Uri("https://api.weather.gov/");
                client.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("weather-tool", "1.0"));
            });

            builder.Services.AddAzureClients(azureBuilder =>
            {
                var configuration = builder.Configuration;
                var connectionString = configuration.GetSection("AzureTableStorage");
                azureBuilder.AddTableServiceClient(connectionString);
            });


            builder.Services.AddSingleton(provider =>
            {
                 var serviceClient = provider.GetRequiredService<Azure.Data.Tables.TableServiceClient>();
                var tableClient = serviceClient.GetTableClient("UserAccounts");
                return tableClient;
            });

            var app = builder.Build();
            app.MapMcp();
            app.Run();
        }
    }
}
