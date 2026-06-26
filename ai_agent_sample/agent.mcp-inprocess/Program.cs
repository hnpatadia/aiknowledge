using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using OpenAI;
using ModelContextProtocol.Client;


var builder = WebApplication.CreateBuilder(args);

// ============================================================================
// 1. SERVICE REGISTRATION
// ============================================================================

builder.Services
    .AddMcpServer()
    .WithHttpTransport(options =>
    {
        options.Stateless = true;
    })
    .WithToolsFromAssembly();

builder.Services.AddSingleton<IChatClient>(sp =>
{
    var apiKey = builder.Configuration["Groq:ApiKey"];

    var openAiClient = new OpenAIClient(
        credential: new System.ClientModel.ApiKeyCredential(apiKey),
        options: new OpenAIClientOptions
        {
            Endpoint = new Uri("https://api.groq.com/openai/v1")
        });

    var chatClient = openAiClient.GetChatClient(
        builder.Configuration["Groq:Model"]);

    return new ChatClientBuilder(chatClient.AsIChatClient())
        .UseFunctionInvocation()
        .Build();
});


builder.Services.AddSingleton<McpClient>( _ =>
{
    var transport = new HttpClientTransport(
        new HttpClientTransportOptions
        {
            Endpoint = new Uri("http://localhost:5180/mcp"),
            TransportMode = HttpTransportMode.AutoDetect
        });

    return McpClient.CreateAsync(transport).Result;
});

var app = builder.Build();

// ============================================================================
// 2. ROUTE MAPPINGS
// ============================================================================
app.MapMcp("mcp");

app.MapPost("/api/agent/ask", async ([FromBody] string prompt, IChatClient aiClient, McpClient mcpClient) =>
{
    var tools = (await mcpClient.ListToolsAsync())
        .Cast<AITool>()
        .ToList();

    var inputSessionContext = new List<ChatMessage> { new ChatMessage(ChatRole.User, prompt) };

    try
    {
        var response = await aiClient.GetResponseAsync(
            inputSessionContext,
            new ChatOptions
            {
                Tools = tools
            });

        return Results.Ok(new { answer = response.Text });


    }
    catch (Exception ex)
    {
        return Results.Problem($"Agent workflow failed execution: {ex.Message}");
    }
});


await app.RunAsync();

