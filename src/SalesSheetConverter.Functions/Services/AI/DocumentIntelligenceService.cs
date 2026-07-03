using Azure;
using Azure.AI.DocumentIntelligence;
using Microsoft.Extensions.Configuration;

namespace SalesSheetConverter.Functions.Services.AI;

public class DocumentIntelligenceService
{
    private readonly DocumentIntelligenceClient _client;

    public DocumentIntelligenceService(IConfiguration configuration)
    {
        var endpoint = configuration["DocumentIntelligence:Endpoint"]!;
        var apiKey = configuration["DocumentIntelligence:ApiKey"]!;

        _client = new DocumentIntelligenceClient(
            new Uri(endpoint),
            new AzureKeyCredential(apiKey));
    }

    public async Task<string> ExtractTextAsync(Stream stream)
    {
        var operation = await _client.AnalyzeDocumentAsync(
            Azure.WaitUntil.Completed,
            "prebuilt-layout",
            BinaryData.FromStream(stream));

        var result = operation.Value;

        return result.Content;
    }
}