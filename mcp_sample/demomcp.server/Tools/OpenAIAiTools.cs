using System.ComponentModel;
using demomcp.server.Services;

namespace demomcp.server.Tools;

using ModelContextProtocol.Server;

[McpServerToolType]
public class OpenAIAiTools
{
    private readonly OpenAIService _openAI;

    public OpenAIAiTools(OpenAIService openAI)
    {
        _openAI = openAI;
    }

    [McpServerTool, Description("Ask My AI")]
    public async Task<string> AskOpenAI(
        string prompt)
    {
        return await _openAI.AskAsync(prompt);
    }

    [McpServerTool, Description("Ask My AI Streaming")]
    public async Task<string> AskOpenAIStreaming(
        string prompt)
    {
        return await _openAI.AskStreamingAsync(prompt);
    }
}