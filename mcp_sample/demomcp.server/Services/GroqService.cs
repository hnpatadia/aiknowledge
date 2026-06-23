
namespace demomcp.server.Services;

public class GroqService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public GroqService(IConfiguration configuration)
    {
        _apiKey = configuration["GROQ_API_KEY"]!;
        _httpClient = new HttpClient();
    }

    public async Task<string> AskAsync(string prompt)
    {
        var request = new
        {
            model = "llama-3.3-70b-versatile",
            messages = new[]
            {
                new
                {
                    role = "user",
                    content = prompt
                }
            }
        };

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add(
            "Authorization",
            $"Bearer {_apiKey}");

        var response = await _httpClient.PostAsJsonAsync(
            "https://api.groq.com/openai/v1/chat/completions",
            request);

        response.EnsureSuccessStatusCode();

        var result =
            await response.Content.ReadFromJsonAsync<GroqResponse>();

        return result!.choices[0].message.content;
    }
}

public class GroqResponse
{
    public List<Choice> choices { get; set; } = [];
}

public class Choice
{
    public Message message { get; set; } = new();
}

public class Message
{
    public string content { get; set; } = "";
}