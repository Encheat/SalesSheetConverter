namespace SalesSheetConverter.Web.Clients;

public interface IConversionApiClient
{
    Task<HttpResponseMessage> ConvertAsync(MultipartFormDataContent content);
}