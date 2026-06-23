
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace demomcp.server.Services;

public class GroqService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly ILogger<GroqService> _logger;

    public GroqService(IConfiguration configuration, ILogger<GroqService> logger)
    {
        _apiKey = configuration["GROQ_API_KEY"]!;
        _httpClient = new HttpClient();
        _logger = logger;
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

        try
        {
            _logger.LogInformation("Sending non-streaming request to Groq API (model={Model}). Prompt length={Length}.", request.model, prompt?.Length ?? 0);

            var response = await _httpClient.PostAsJsonAsync(
                "https://api.groq.com/openai/v1/chat/completions",
                request);

            _logger.LogInformation("Received response from Groq API. StatusCode={StatusCode}", response.StatusCode);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<GroqResponse>();

            var content = result?.choices?.FirstOrDefault()?.message?.content;
            _logger.LogDebug("Groq non-streaming response content length={Length}", content?.Length ?? 0);

            return content ?? string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while calling Groq API (non-streaming)");
            throw;
        }
    }


    public async Task<string> AskStreamingAsync(string prompt)
    {
        var request = new
        {
            model = "llama-3.3-70b-versatile",
            stream = true,
            messages = new[]
            {
                new
                {
                    role = "user",
                    content = prompt
                }
            }
        };

        using var httpRequest = new HttpRequestMessage(
            HttpMethod.Post,
            "https://api.groq.com/openai/v1/chat/completions");

        httpRequest.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", _apiKey);

        httpRequest.Content = JsonContent.Create(request);

        using var response = await _httpClient.SendAsync(
            httpRequest,
            HttpCompletionOption.ResponseHeadersRead);

        try
        {
            _logger.LogInformation("Sending streaming request to Groq API (model={Model}). Prompt length={Length}.", request.model, prompt?.Length ?? 0);

            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);

            var result = new StringBuilder();

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (!line.StartsWith("data:"))
                    continue;

                var json = line["data:".Length..].Trim();

                if (json == "[DONE]")
                    break;

                try
                {
                    using var doc = JsonDocument.Parse(json);

                    var root = doc.RootElement;

                    if (root.TryGetProperty("choices", out var choices))
                    {
                        var delta = choices[0]
                            .GetProperty("delta");

                        if (delta.TryGetProperty("content", out var content))
                        {
                            var chunk = content.GetString();
                            if (!string.IsNullOrEmpty(chunk))
                            {
                                result.Append(chunk);
                                _logger.LogInformation("Streaming chunk received:  (data={data}).", chunk);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Malformed streaming chunk ignored.");
                }
            }

            _logger.LogInformation("Streaming completed. Total length={Length}.", result.Length);

            return result.ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while calling Groq API (streaming)");
            throw;
        }
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