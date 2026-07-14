using System.Net.Http.Headers;
using Microsoft.JSInterop;
using SalesSheetConverter.Web.Clients;
using SalesSheetConverter.Web.Models;

namespace SalesSheetConverter.Web.Services;

public interface IUploadService
{
    public Task<string> Upload(List<UploadedFile> files);
}

public class UploadService : IUploadService
{
    private readonly IConversionApiClient _conversionApiClient;
    private readonly IJSRuntime _jsRuntime;

    public UploadService(IConversionApiClient conversionApiClient, IJSRuntime jsRuntime)
    {
        _conversionApiClient = conversionApiClient;
        _jsRuntime = jsRuntime;
    }

    public async Task<string> Upload(List<UploadedFile> files)
    {
        var _result = "";
        try{
            using var content = new MultipartFormDataContent();

            foreach (var file in files)
            {
                var fileContent = new ByteArrayContent(file.Bytes);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                content.Add(fileContent, "files", file.Name);
            }

            var response = await _conversionApiClient.ConvertAsync(content);

            if (!response.IsSuccessStatusCode)
            {
                //TODO: Improve returned value
                // _result = "An issue has occurred.";
                _result = await response.Content.ReadAsStringAsync();
                return _result;
            }

            var contentDisposition = response.Content.Headers.ContentDisposition;
            var fileName = contentDisposition?.FileNameStar?.Trim('"')
                ?? contentDisposition?.FileName?.Trim('"')
                ?? "Sales.csv";

            var bytes = await response.Content.ReadAsByteArrayAsync();
            var base64 = Convert.ToBase64String(bytes);

            await _jsRuntime.InvokeVoidAsync("downloadFile", fileName, "text/csv", base64);
            _result = "Download complete.";
            return _result;
        }
        catch (Exception ex)
        {
            //TODO: prevent throwing errors with too much detail to users.
            _result = $"Upload failed: {ex.Message}";
            return _result;
        }
        //TODO: Add 'log error' button that captures the _result and sends a notification with the revelevant
        // info so I don't have to dig through logs to see what happened.
    }
}