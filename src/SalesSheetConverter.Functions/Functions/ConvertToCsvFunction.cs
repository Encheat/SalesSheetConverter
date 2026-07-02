using SalesSheetConverter.Functions.Services;
using SalesSheetConverter.Shared.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.AspNetCore.Mvc;
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
public async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
{
    // For file uploads in isolated functions, read from body
    using var stream = req.Body;
    
    _logger.LogInformation("Received file upload");

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
    };

    var csv = _csvExportService.CreateCsv(sales);
    return new FileContentResult(csv, "text/csv")
    {
        FileDownloadName = "Sales.csv"
    };
}
}