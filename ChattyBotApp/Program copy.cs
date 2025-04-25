using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ChattyBotApp;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRouting();
var app = builder.Build();

app.UseStaticFiles();

// Serve index.html
app.MapGet("/", () => Results.Redirect("/index.html"));

// POST /chat
app.MapPost("/chat", async (HttpRequest request) =>
{
    var data = await request.ReadFromJsonAsync<ChatRequest>();
    var userMessage = data.Messages.Last().Content;

    // 1. Exact Match
    foreach (var item in ChattyFAQ.Data)
    {
        if (userMessage.Contains(item.Key, StringComparison.OrdinalIgnoreCase))
            return Results.Json(new { reply = item.Value });
    }

    // 2. Suggestion Logic
    var suggestions = ChattyFAQ.Data.Keys
        .Where(key => key.Contains(userMessage, StringComparison.OrdinalIgnoreCase)
                   || userMessage.Contains(key, StringComparison.OrdinalIgnoreCase))
        .ToList();

    if (suggestions.Any())
    {
        string suggested = suggestions.First();
        return Results.Json(new
        {
            reply = $"คุณหมายถึง \"{suggested}\" หรือไม่?\nคำตอบ: {ChattyFAQ.Data[suggested]}"
        });
    }

    // 3. Ask ChatGPT
    var chatGptResponse = await OpenAIService.AskChatGPT(userMessage);
    return Results.Json(new { reply = chatGptResponse });
});

// POST /upload
app.MapPost("/upload", async (IFormFile file) =>
{
    return Results.Json(new
    {
        message = $"Received file: {file.FileName}, size: {file.Length} bytes"
    });
});

app.Run();

// Support record types
record ChatRequest(List<Message> Messages);
record Message(string Role, string Content);
