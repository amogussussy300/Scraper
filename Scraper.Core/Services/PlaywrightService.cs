using Microsoft.Extensions.Caching.Memory;
using Microsoft.Playwright;

namespace Scraper.Core.Services;

public interface IPlaywrightService : IDisposable
{
    Task<string?> GetHtmlAsync(string url, string cacheKey);
    void InvalidateCache(string cacheKey);
}

public class PlaywrightService : IPlaywrightService
{
    private readonly IMemoryCache _cache;
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private const string CachePrefix = "html_";

    public PlaywrightService(IMemoryCache cache) => _cache = cache;

    private async Task<IPlaywright> GetPlaywrightAsync()
    {
        _playwright ??= await Microsoft.Playwright.Playwright.CreateAsync();
        return _playwright;
    }

    private async Task<IBrowser> GetBrowserAsync()
    {
        if (_browser == null || !_browser.IsConnected)
        {
            var pw = await GetPlaywrightAsync();
            _browser = await pw.Chromium.LaunchAsync();
        }
        return _browser;
    }

    public async Task<string?> GetHtmlAsync(string url, string cacheKey)
    {
        var cached = _cache.Get<string>($"{CachePrefix}{cacheKey}");
        if (!string.IsNullOrEmpty(cached))
            return cached;

        var browser = await GetBrowserAsync();
        var page = await browser.NewPageAsync();
        try
        {
            await page.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded });
            var html = await page.ContentAsync();
            _cache.Set($"{CachePrefix}{cacheKey}", html, TimeSpan.FromMinutes(30));
            return html;
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    public void InvalidateCache(string cacheKey)
    {
        _cache.Remove($"{CachePrefix}{cacheKey}");
    }

    public void Dispose()
    {
        try
        {
            _browser?.CloseAsync().GetAwaiter().GetResult();
        }
        catch
        {

        }
        _playwright?.Dispose();
        GC.SuppressFinalize(this);
    }
}