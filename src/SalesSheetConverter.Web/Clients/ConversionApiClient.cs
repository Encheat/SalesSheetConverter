namespace SalesSheetConverter.Web.Clients;

public class ConversionApiClient : IConversionApiClient
{
    private readonly HttpClient _httpClient;

    public ConversionApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HttpResponseMessage> ConvertAsync(MultipartFormDataContent content)
    {
        return await _httpClient.PostAsync("/api/ConvertToCsv", content);
    }
}