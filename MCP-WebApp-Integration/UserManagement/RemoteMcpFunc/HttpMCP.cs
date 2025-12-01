using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Mcp.Function;

public class HttpMCP
{
    private readonly ILogger<HttpMCP> _logger;

    public HttpMCP(ILogger<HttpMCP> logger)
    {
        _logger = logger;
    }

    [Function("HttpMCP")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions!");
    }
}