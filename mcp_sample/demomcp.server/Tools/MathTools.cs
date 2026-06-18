using ModelContextProtocol.Server;

namespace DemoMcp.Server.Tools;

[McpServerToolType]
public static class MathTools
{
    [McpServerTool]
    public static int Add(int a, int b)
    {
        return a + b;
    }

    [McpServerTool]
    public static int Multiply(int a, int b)
    {
        return a * b;
    }
}