using Azure.Storage.Blobs;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;
using OpenAI.Embeddings;
using UserManagementService;

namespace UserManagement.Api
{
    //Note: To run the MCP Inspector for this server, use the following command:
    //npx @modelcontextprotocol/inspector
    public class Program
    {
        static string GetConfigurationValue(WebApplicationBuilder webApplicationBuilder, string key)
        {
            var configurationValue = webApplicationBuilder.Configuration[key];
            if (string.IsNullOrEmpty(configurationValue))
                return Environment.GetEnvironmentVariable(key) ?? throw new InvalidOperationException($"Configuration key '{key}' not found in environment variables.");
            return configurationValue;
        }
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSingleton<StorageAccountTableService.StorageAccountTableService>();
            builder.Services.AddSingleton<StorageAccountBlobService>();

            builder.Services.AddMcpServer().WithHttpTransport().WithTools<Tools.AzureSearchTools>();
            builder.Services.AddMcpServer().WithHttpTransport().WithTools<Tools.WeatherAlertsTool>();
            builder.Services.AddMcpServer().WithHttpTransport().WithTools<Tools.UserManagementTools>();
            builder.Services.AddMcpServer().WithHttpTransport().WithTools<Tools.InsurancePolicyTools>();




            builder.Services.AddHttpClient("WeatherApi", client =>
            {
                client.BaseAddress = new Uri("https://api.weather.gov/");
                client.DefaultRequestHeaders.UserAgent.Add(new System.Net.Http.Headers.ProductInfoHeaderValue("weather-tool", "1.0"));
            });

            builder.Services.AddTransient(serviceProvider =>
            {
                var azure_open_ai_key = GetConfigurationValue(builder, "AZURE_OPEN_API_KEY");
                var azure_open_ai_endpoint = GetConfigurationValue(builder, "AZURE_OPEN_API_ENDPOINT");
                var client = new OpenAIClient(new Azure.AzureKeyCredential(azure_open_ai_key)
                , new OpenAIClientOptions() { Endpoint = new Uri($"{azure_open_ai_endpoint}/models") });
                var embeddingClient = client.GetEmbeddingClient("text-embedding-ada-002");
                return embeddingClient;
            });

            builder.Services.AddTransient(serviceProvider =>
            {
                var azure_search_endpoint = GetConfigurationValue(builder, "AZURE_SEARCH_ENDPOINT");
                var azure_search_key = GetConfigurationValue(builder, "AZURE_SEARCH_KEY");
                var searchClient = new Azure.Search.Documents.SearchClient(new Uri(azure_search_endpoint), "insurance-claim-index", new Azure.AzureKeyCredential(azure_search_key));
                return searchClient;
            });
            builder.Services.AddTransient(serviceProvider =>
            {
                var connectionString = GetConfigurationValue(builder, "AzureWebBlobStorage");
                return new BlobContainerClient(connectionString, "rag-documents");
            });
            builder.Services.AddAzureClients(azureBuilder =>
                   {
                       var connectionString = GetConfigurationValue(builder, "AzureTableStorage");
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
