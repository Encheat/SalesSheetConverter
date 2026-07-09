namespace SalesSheetConverter.Web.Clients;

public class ConversionApiClient : IConversionApiClient
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public ConversionApiClient(IConfiguration configuration, HttpClient httpClient)
    {
        _configuration = configuration;
        _httpClient = httpClient;
    }

    public async Task<HttpResponseMessage> ConvertAsync(MultipartFormDataContent content)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/ConvertToCsv")
        {
            Content = content
        };

        var isLocal = _httpClient.BaseAddress?.Host.Contains("localhost", StringComparison.OrdinalIgnoreCase) == true;
        if (!isLocal)
        {
            var key = _configuration["FunctionsApi:Key"] ?? _configuration["FunctionsApi__Key"];
            if (!string.IsNullOrWhiteSpace(key))
            {
                request.Headers.Add("x-functions-key", key);
            }
        }

        return await _httpClient.SendAsync(request);
    }
}