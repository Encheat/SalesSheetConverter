using SalesSheetConverter.Web.Components;

var builder = WebApplication.CreateBuilder(args);

//Make sure local settings are added if they exist, ignore if not
builder.Configuration
    .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddHttpClient("FunctionsApi", client =>
{
    var baseUrl = builder.Configuration["FunctionsApi:BaseUrl"];

    if (string.IsNullOrWhiteSpace(baseUrl))
    {
        throw new InvalidOperationException("FunctionsApi:BaseUrl must be configured.");
    }

    client.BaseAddress = new Uri(baseUrl);
});

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
