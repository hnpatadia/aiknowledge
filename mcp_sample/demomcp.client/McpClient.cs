using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

public class McpClient
{
    private readonly HttpClient _http;
    private readonly string _endpoint;

    public McpClient(string endpoint)
    {
        _endpoint = endpoint;
        _http = new HttpClient();
    }

    public async Task<string> ListTools()
    {
        var request = new
        {
            jsonrpc = "2.0",
            id = Guid.NewGuid().ToString(),
            method = "tools/list"
        };

        return await Send(request);
    }

    public async Task<string> CallTool(
        string toolName,
        object arguments)
    {
        var request = new
        {
            jsonrpc = "2.0",
            id = Guid.NewGuid().ToString(),
            method = "tools/call",
            @params = new
            {
                name = toolName,
                arguments
            }
        };

        return await Send(request);
    }

    private async Task<string> Send(object request)
    {
        var json = JsonSerializer.Serialize(request);


        using var httpRequest = new HttpRequestMessage(
            HttpMethod.Post,
            _endpoint);

        httpRequest.Content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json");

        httpRequest.Headers.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        httpRequest.Headers.Accept.Add(
            new MediaTypeWithQualityHeaderValue("text/event-stream"));


        var response = await _http.SendAsync(httpRequest);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
}