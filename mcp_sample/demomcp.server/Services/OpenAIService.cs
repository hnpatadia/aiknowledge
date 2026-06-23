using OpenAI;
using OpenAI.Chat;
using System.Text;

namespace demomcp.server.Services;

public class OpenAIService
{
    private readonly ChatClient _chatClient;

    public OpenAIService(IConfiguration config)
    {
        var apiKey = config["OPENAI_API_KEY"];

        var client = new OpenAIClient(apiKey);

        _chatClient = client.GetChatClient("gpt-5");
    }

    public async Task<string> AskAsync(string prompt)
    {
        var response = await _chatClient.CompleteChatAsync(
        [
            new UserChatMessage(prompt)
        ]);

        return response.Value.Content[0].Text;
    }

    public async Task<string> AskStreamingAsync(string prompt)
    {
        StringBuilder sb = new();

        await foreach (var update in _chatClient.CompleteChatStreamingAsync(prompt))
        {
            foreach (var part in update.ContentUpdate)
            {
                sb.Append(part.Text);
            }
        }

        return sb.ToString();
    }
}