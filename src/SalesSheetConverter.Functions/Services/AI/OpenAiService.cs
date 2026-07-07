using System.ClientModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenAI;
using OpenAI.Responses;
using SalesSheetConverter.Shared.Models;

namespace SalesSheetConverter.Functions.Services.AI;

public class OpenAIService
{
    #pragma warning disable OPENAI001 //I don't like this but I'd rather use future-supported features than older patterns.
    private readonly ILogger<OpenAIService> _logger;
    private readonly ResponsesClient _responsesClient;

    private readonly string _deployment;
    private readonly bool _isLocal;

    public OpenAIService(ILogger<OpenAIService> logger, IConfiguration configuration, bool isLocal)
    {
        #pragma warning disable OPENAI001
        _logger = logger;
        var endpoint = configuration["OpenAI:Endpoint"]!;
        var apiKey = configuration["OpenAI:ApiKey"]!;
        _deployment = configuration["OpenAI:Deployment"]!;
        _isLocal = isLocal;

       var client = new OpenAIClient(
            credential: new ApiKeyCredential(apiKey),
            options: new OpenAIClientOptions
            {
                Endpoint = new Uri(endpoint)
            });

        _responsesClient = client.GetResponsesClient();
    }

    public async Task<string> ExtractJsonAsync(
        IEnumerable<ImageDetails> images, 
        string promptName = "SalesExtraction", 
        CancellationToken cancellationToken = default)
    {
        var contentParts = new List<ResponseContentPart>
        {
            ResponseContentPart.CreateInputTextPart(LoadPrompt(promptName))
        };

        foreach (var image in images)
        {
            // using var memory = new MemoryStream();
            // await image.Stream.CopyToAsync(memory, cancellationToken);
            // var bytes = BinaryData.FromBytes(memory.ToArray(), image.ContentType);
            var bytes = BinaryData.FromStream(image.Stream, image.ContentType);
            var content = ResponseContentPart.CreateInputImagePart(bytes);

            contentParts.Add(content);
        }

        CreateResponseOptions options = new()
        {
            Model = _deployment,
            InputItems =
            {
                ResponseItem.CreateUserMessageItem(contentParts),
            },
        };

        ResponseResult response = new();

        try
        {
            response = await _responsesClient.CreateResponseAsync(options, cancellationToken);
        }
        catch(Exception e)
        {
            _logger.LogError(e, "Exception occured when calling AI endpoint.");
        }

        if (_isLocal)
        {
            Console.WriteLine($"[ASSISTANT]: {response.GetOutputText()}");
        }

        return response.GetOutputText();
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
        var blankSalesExtractionResult = new ExtractionResult();
        return JsonSerializer.Serialize(blankSalesExtractionResult);
    }
}