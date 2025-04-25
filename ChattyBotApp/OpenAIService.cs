using Azure;
using Azure.AI.OpenAI;
using DotNetEnv;
using System;
using System.Threading.Tasks;

public static class OpenAIService
{
    
    static OpenAIService()
    {
        Env.Load();
    }

    public static async Task<string> AskChatGPT(string userMessage)
    {
        var apiKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_KEY");
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("AZURE_OPENAI_KEY is not set in environment variables or .env file.");
        }

        // Initialize OpenAIClient correctly
        var client = new OpenAIClient(
            new Uri("https://mju64-m9usb3dg-eastus2.cognitiveservices.azure.com/"),
            new AzureKeyCredential(apiKey)
        );

        // Prepare chat completion options
        var chatOptions = new ChatCompletionsOptions()
        {
            Messages =
            {
                new ChatMessage(ChatRole.System, "You are a helpful assistant."),
                new ChatMessage(ChatRole.User, userMessage)
            }
        };

        // Call Azure OpenAI
        Response<ChatCompletions> response = await client.GetChatCompletionsAsync(
            "Chatty",  // Deployment name
            chatOptions
        );

        return response.Value.Choices[0].Message.Content;
    }
}
