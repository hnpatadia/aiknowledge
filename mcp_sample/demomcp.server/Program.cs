
using demomcp.server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMcpServer()
    .WithHttpTransport(options =>
    {
        options.Stateless = true;
    })
    .WithToolsFromAssembly();

builder.Services.AddSingleton<GroqService>();
builder.Services.AddSingleton<OpenAIService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("mcp", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.MapMcp("mcp");

app.Run();