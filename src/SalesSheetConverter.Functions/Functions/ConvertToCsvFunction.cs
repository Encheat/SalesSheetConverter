using System.Net;
using SalesSheetConverter.Functions.Services;
using SalesSheetConverter.Shared.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

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
        if (!req.Headers.TryGetValues("Content-Type", out var contentTypes))
        {
            var bad = req.CreateResponse(HttpStatusCode.BadRequest);
            await bad.WriteStringAsync("Missing Content-Type header.");
            return bad;
        }

        var contentType = contentTypes.FirstOrDefault();
        var mediaType = MediaTypeHeaderValue.Parse(contentType);
        var boundary = HeaderUtilities.RemoveQuotes(mediaType.Boundary).Value;
        if (string.IsNullOrEmpty(boundary))
        {
            var bad = req.CreateResponse(HttpStatusCode.BadRequest);
            await bad.WriteStringAsync("Invalid multipart boundary.");
            return bad;
        }

        var reader = new MultipartReader(boundary, req.Body);
        var section = await reader.ReadNextSectionAsync();
        var fileCount = 0;

        while (section != null)
        {
            var contentDisposition = ContentDispositionHeaderValue.Parse(section.ContentDisposition);
            if (contentDisposition.DispositionType.Equals("form-data") &&
                !string.IsNullOrEmpty(contentDisposition.FileName.Value))
            {
                fileCount++;
                // process file section here
            }

            section = await reader.ReadNextSectionAsync();
        }

        if (fileCount == 0)
        {
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("No file uploaded.");
            return badResponse;
        }

        // ... create CSV and return response ...

        var sales = new List<SaleRecord>
        {
            new()
            {
                Date = DateOnly.FromDateTime(DateTime.Today),
                Item = "Widget A",
                Quantity = 2,
                UnitPrice = 9.99m,
                TotalPrice = 19.98m
            }
            // foreach (var file in parser.Files)
            // {
            //     using var stream = file.OpenReadStream();

            //     var extractedSales = await _documentIntelligenceService.ExtractSalesAsync(stream);

            //     sales.AddRange(extractedSales);
            // }
        };

        var csv = _csvExportService.CreateCsv(sales);

        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "text/csv");
        response.Headers.Add("Content-Disposition", "attachment; filename=Sales.csv");
        await response.WriteBytesAsync(csv);

        return response;
    }
}