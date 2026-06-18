using ModelContextProtocol.Server;

namespace DemoMcp.Server.Tools;

[McpServerToolType]
public static class HelloTools
{
    [McpServerTool]
    public static string Hello(string name)
    {
        return $"Hello {name}";
    }
}