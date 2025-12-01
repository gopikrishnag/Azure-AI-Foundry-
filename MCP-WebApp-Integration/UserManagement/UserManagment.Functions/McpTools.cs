using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Mcp;
using Microsoft.Extensions.Logging;

namespace UserManagment.Functions;

public class McpTools
{
    private readonly ILogger<McpTools> _logger;

    public McpTools(ILogger<McpTools> logger)
    {
        _logger = logger;
    }

    [Function("McpTools")]
    public string Run([McpToolTrigger("MCP Server status", "This is the endpoint for check the server status")] ToolInvocationContext toolInvocationContext )
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return "MCP server works well";
    }
}