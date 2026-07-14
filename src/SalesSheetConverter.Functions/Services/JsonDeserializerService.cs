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
        var result = null as ExtractionResult;
        try
        {
            //TODO: improve json verification. Potentially add schema validation/JsonSerializerSettings' Error to log more details.
            result = JsonSerializer.Deserialize<ExtractionResult>(json, Options);
            if (result is null ||
                result.CsvLines is null ||
                result.CsvLines.Count == 0 ||
                result.CsvLines.FirstOrDefault()?.Trim().Length == 0)
            {
                throw new InvalidOperationException($"The json deserialized into an invalid object: {result}");
            }
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"The json could not be deserialized: {json}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"An unexpected error occurred while deserializing the json: {json}", ex);
        }
        
        return result;
    }
}