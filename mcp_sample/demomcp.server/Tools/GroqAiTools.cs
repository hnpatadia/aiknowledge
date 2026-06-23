using demomcp.server.Services;
using ModelContextProtocol.Server;
using System.ComponentModel;

[McpServerToolType]
public sealed class GroqAiTools
{
    private readonly GroqService _groq;

    public GroqAiTools(GroqService groq)
    {
        _groq = groq;
    }

    [McpServerTool]
    [Description("Ask an Groq AI model")]
    public async Task<string> AskGroqAI(string prompt)
    {
        return await _groq.AskAsync(prompt);
    }
}