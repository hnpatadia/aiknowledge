using ModelContextProtocol.Server;

namespace DemoMcp.Server.Tools;

[McpServerToolType]
public static class SystemTools
{
    [McpServerTool]
    public static string CurrentTime()
    {
        return DateTime.UtcNow.ToString("O");
    }

    [McpServerTool]
    public static string ServerInfo()
    {
        return Environment.MachineName;
    }
}