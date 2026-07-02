using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Company.Function;

public class TriggerSalesSheetConverter
{
    private readonly ILogger<TriggerSalesSheetConverter> _logger;

    public TriggerSalesSheetConverter(ILogger<TriggerSalesSheetConverter> logger)
    {
        _logger = logger;
    }

    [Function("TriggerSalesSheetConverter")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult("Welcome to Azure Functions!");
    }
}