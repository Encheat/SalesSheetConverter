using System.ClientModel;
using System.Text.Json;
using Azure;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Responses;
using SalesSheetConverter.Shared.Models;

namespace SalesSheetConverter.Functions.Services.AI;

public class OpenAIService
{
    private readonly ChatClient _chatClient;

    public OpenAIService(IConfiguration configuration)
    {
        var endpoint = configuration["OpenAI:Endpoint"]!;
        var apiKey = configuration["OpenAI:ApiKey"]!;
        var deployment = configuration["OpenAI:Deployment"]!;

       var client = new OpenAIClient(
            credential: new ApiKeyCredential(apiKey),
            options: new OpenAIClientOptions
            {
                Endpoint = new Uri(endpoint)
            });

        _chatClient = client.GetChatClient(deployment);
    }

    public async Task<string> ExtractJsonAsync(
        IEnumerable<ImageDetails> images, 
        string promptName = "SalesExtraction", 
        CancellationToken cancellationToken = default)
    {
        var prompt = LoadPrompt(promptName);

        var messages = new List<ChatMessage>
        {
            ChatMessage.CreateSystemMessage(prompt)
        };

        foreach (var image in images)
        {
            using var memory = new MemoryStream();
            await image.Stream.CopyToAsync(memory, cancellationToken);

            var content = new List<ChatMessageContentPart>
            {
                ChatMessageContentPart.CreateImagePart(
                    BinaryData.FromBytes(memory.ToArray()),
                    image.ContentType)
            };
            //Adding it to the list of messages to be sent.
            messages.Add(ChatMessage.CreateUserMessage(content));
        }

        var response = await _chatClient.CompleteChatAsync(messages, cancellationToken: cancellationToken);

        return string.Concat(
            response.Value.Content
                .Where(c => !string.IsNullOrWhiteSpace(c.Text))
                .Select(c => c.Text));
    }

    private static string LoadPrompt(string promptName)
    {
        var prompt = File.ReadAllText(
            Path.Combine(
                AppContext.BaseDirectory,
                "Services",
                "AI",
                "Prompts",
                $"{promptName}Prompt.txt"));
        
        prompt += $"\n{LoadSchema()}";
        return prompt;
    }

    //TODO: we may make this dependent on promptName
    private static string LoadSchema()
    {
        var blankSalesExtractionResult = new SalesExtractionResult();
        blankSalesExtractionResult.ColumnValueEntries.Add(new Row());
        return JsonSerializer.Serialize(blankSalesExtractionResult);
    }
}