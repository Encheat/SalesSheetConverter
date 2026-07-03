using System.Net;
using SalesSheetConverter.Functions.Services;
using SalesSheetConverter.Shared.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Http;

namespace SalesSheetConverter.Functions;

public class ConvertToCsvFunction
{
    private readonly ILogger<ConvertToCsvFunction> _logger;
    private readonly CsvExportService _csvExportService;

    public ConvertToCsvFunction(ILogger<ConvertToCsvFunction> logger, CsvExportService csvExportService)
    {
        _logger = logger;
        _csvExportService = csvExportService;
    }

    [Function("ConvertToCsv")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {
        try
        {
            if (!req.Headers.TryGetValues("Content-Type", out var contentTypes))
            {
                var bad = req.CreateResponse(HttpStatusCode.BadRequest);
                await bad.WriteStringAsync("Missing Content-Type header.");
                return bad;
            }

            var contentType = contentTypes.FirstOrDefault();
            var mediaType = MediaTypeHeaderValue.Parse(contentType);
            
            if (mediaType.Boundary.Length == 0)
            {
                var bad = req.CreateResponse(HttpStatusCode.BadRequest);
                await bad.WriteStringAsync("Invalid multipart boundary.");
                return bad;
            }

            var boundary = HeaderUtilities.RemoveQuotes(mediaType.Boundary).Value;
            if (boundary == null)
            {
                var bad = req.CreateResponse(HttpStatusCode.BadRequest);
                await bad.WriteStringAsync("Empty boundary entry value.");
                return bad;
            }
            _logger.LogInformation($"Parsing multipart with boundary: {boundary}");

            var sales = new List<SaleRecord>();
            var reader = new MultipartReader(boundary, req.Body);

            MultipartSection? section = await reader.ReadNextSectionAsync();
            while (section != null)
            {
                var contentDisposition = ContentDispositionHeaderValue.Parse(section.ContentDisposition);
                _logger.LogInformation($"Section found: {contentDisposition.FileName}");

                if (contentDisposition.DispositionType.Equals("form-data") &&
                    !string.IsNullOrEmpty(contentDisposition.FileName.Value))
                {
                    using var memoryStream = new MemoryStream();
                    await section.Body.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    _logger.LogInformation($"Uploaded file: {contentDisposition.FileName.Value}, Size: {memoryStream.Length} bytes");

                    // TODO: replace this placeholder with actual extraction logic
                    // var extractedSales = await _documentIntelligenceService.ExtractSalesAsync(memoryStream);
                    // sales.AddRange(extractedSales);
                }

                section = await reader.ReadNextSectionAsync();
            }

            _logger.LogInformation($"Total sections processed, sales count: {sales.Count}");

            //PLACEHOLDER
            sales = new List<SaleRecord>
            {
                new()
                {
                    Date = DateOnly.FromDateTime(DateTime.Today),
                    Item = "Widget A",
                    Quantity = 2,
                    UnitPrice = 9.99m,
                    TotalPrice = 19.98m
                }
            };

            var csv = _csvExportService.CreateCsv(sales);

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/csv");
            response.Headers.Add("Content-Disposition", "attachment; filename=Sales.csv");
            await response.WriteBytesAsync(csv);

            return response;
        }
        catch (BadHttpRequestException ex)
        {
            _logger.LogError($"Multipart parsing failed: {ex.Message}");
            var response = req.CreateResponse(HttpStatusCode.BadRequest);
            await response.WriteStringAsync($"Invalid multipart request: {ex.Message}");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unexpected error: {ex.Message}");
            var response = req.CreateResponse(HttpStatusCode.InternalServerError);
            await response.WriteStringAsync($"Error processing request: {ex.Message}");
            return response;
        }
    }
}