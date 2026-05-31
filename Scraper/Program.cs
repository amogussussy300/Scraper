using Scraper.Components;
using Scraper.Core;
using Scraper.Parsing.Services;
using Scraper.Services;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

Startup.ConfigureServices(builder.Services, builder.Configuration);

builder.Services.AddSingleton<IPlaywrightService, PlaywrightService>();
builder.Services.AddSingleton<ParsingService>();
builder.Services.AddSingleton<PageStore>();

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.Run();