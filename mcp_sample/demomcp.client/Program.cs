var client = new McpClient("http://localhost:5179/mcp");

Console.WriteLine("=== TOOLS ===");


try
{
    var toolsResponse = await client.ListTools();
    Console.WriteLine(toolsResponse);

    Console.WriteLine();

    Console.WriteLine("=== HELLO ===");
    var response = await client.CallTool(
        "hello",
        new { name = "Hitesh" });
    Console.WriteLine(response);
    Console.WriteLine();

    Console.WriteLine("=== ADD ===");

    var response1 = await client.CallTool(
        "add",
        new { a = 10, b = 20 });
    Console.WriteLine(response1);
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}

Console.Write("Ending now......");
Console.ReadLine();