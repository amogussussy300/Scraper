using Microsoft.Playwright;
namespace Scraper.Parsing.Services;
public interface IPlaywrightService : IAsyncDisposable
{
    Task<string?> GetHtmlAsync(string url, CancellationToken ct = default);
}
public class PlaywrightService : IPlaywrightService
{
    private const string DefaultUserAgent =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
        "(KHTML, like Gecko) Chrome/134.0.0.0 Safari/537.36";
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IBrowserContext? _context;
    private readonly SemaphoreSlim _initLock = new(1, 1);
    private async Task<IBrowserContext> GetContextAsync()
    {
        if (_context is not null && _browser is { IsConnected: true })
            return _context;
        await _initLock.WaitAsync();
        try
        {
            if (_context is not null && _browser is { IsConnected: true })
                return _context;
            _playwright ??= await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true
            });
            _context = await _browser.NewContextAsync(new BrowserNewContextOptions
            {
                UserAgent = DefaultUserAgent
            });
            return _context;
        }
        finally
        {
            _initLock.Release();
        }
    }
    private static string NormalizeUrl(string url)
    {
        var trimmed = url.Trim();
        if (!trimmed.Contains("://", StringComparison.Ordinal))
            trimmed = "https://" + trimmed;
        return trimmed;
    }

    public async Task<string?> GetHtmlAsync(string url, CancellationToken ct = default)
    {
        var context = await GetContextAsync();
        var page = await context.NewPageAsync();
        try
        {
            await page.GotoAsync(NormalizeUrl(url), new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded,
                Timeout = 60_000
            });
            return await page.ContentAsync();
        }
        finally
        {
            await page.CloseAsync();
        }
    }
    public async ValueTask DisposeAsync()
    {
        if (_context is not null) await _context.CloseAsync();
        if (_browser is not null) await _browser.CloseAsync();
        _playwright?.Dispose();
        GC.SuppressFinalize(this);
    }
}