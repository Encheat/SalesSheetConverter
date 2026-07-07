using System.Text.Json;
using SalesSheetConverter.Shared.Models;

namespace SalesSheetConverter.Functions.Services;

public sealed class JsonDeserializationService
{
    private static readonly JsonSerializerOptions Options =
        new()
        {
            PropertyNameCaseInsensitive = true
        };

    public ExtractionResult Parse(string json)
    {
        var result = JsonSerializer.Deserialize<ExtractionResult>(json, Options);

        if (result is null)
        {
            throw new InvalidOperationException($"The json could not be deserialized into a SalesExtractionResult object: {json}");
        }

        return result;
    }
}