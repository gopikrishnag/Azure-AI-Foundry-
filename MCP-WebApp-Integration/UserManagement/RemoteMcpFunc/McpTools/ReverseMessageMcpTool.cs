

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Mcp;

namespace Mcp.McpTools;

public class ReverseMessageMcpTool {
  [Function("ReverseMessageMcpTool")]
  public IActionResult Run(
    [McpToolTrigger("ReverseMessageTool", "Echoes back message in reverse.")]
    ToolInvocationContext context,
    [McpToolProperty("Message", "The Message to reverse")]
    string message
  ) {
    string reversedMessage = new string(message.ToCharArray().Reverse().ToArray());
    return new OkObjectResult($"Hi. I'm  ReverseMessageTool !. The reversed message is: {reversedMessage}");
  }
}