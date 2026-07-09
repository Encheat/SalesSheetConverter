using SalesSheetConverter.Web.Clients;
using SalesSheetConverter.Web.Components;
using SalesSheetConverter.Web.Services;
using SalesSheetConverter.Web.Spinner;

var builder = WebApplication.CreateBuilder(args);

//Make sure local settings are added if we're local
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);
}

// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddHttpClient<IConversionApiClient, ConversionApiClient>(client =>
{
    var baseUrl = builder.Configuration["FunctionsApi--BaseUrl"];

    if (string.IsNullOrWhiteSpace(baseUrl))
    {
        throw new InvalidOperationException(
            "Functions API base URL was not configured. Set FunctionsApi--BaseUrl in Azure App Service settings.");
    }

    client.BaseAddress = new Uri(baseUrl);
});
builder.Services.AddScoped<IUploadService, UploadService>();
builder.Services.AddScoped<ISpinnerProvider, SpinnerProvider>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
