using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Scraper.Core.Dtos;
using Scraper.Parsing.Services;
namespace Scraper.Components;

public partial class EditActionOverlay
{
    [Parameter] public int? ActionIndex { get; set; }      // null = closed
    [Parameter] public ActionDto? Action { get; set; }     // копия, переданная из PageEditor
    [Parameter] public string PageUrl { get; set; } = "";
    [Parameter] public EventCallback OnClose { get; set; }
    [Parameter] public EventCallback<ActionDto> OnApplied { get; set; }
    [Inject] private IPlaywrightService PlaywrightService { get; set; } = default!;
    [Inject] private ParsingService ParsingService { get; set; } = default!;
    private ActionDto? local;
    private string rawHtml = "";
    private List<string> previewValues = new();
    private bool isTesting;
    private string? errorMessage;
    private bool visible;
    private int? loadedForIndex;
    private CancellationTokenSource? _testCts;
    private const int HtmlPreviewLimit = 50_000;
    private string TruncatedHtml =>
        rawHtml.Length > HtmlPreviewLimit
            ? rawHtml.Substring(0, HtmlPreviewLimit) + $"\n\n... [truncated, total {rawHtml.Length} chars]"
            : rawHtml;
    protected override void OnParametersSet()
    {
        if (ActionIndex.HasValue && Action != null && ActionIndex != loadedForIndex)
        {
            // Берём ещё одну копию, чтобы Cancel в overlay не задел копию parent'а
            var json = JsonSerializer.Serialize(Action);
            local = JsonSerializer.Deserialize<ActionDto>(json);
            loadedForIndex = ActionIndex;
            errorMessage = null;
            rawHtml = "";
            previewValues = new();
            visible = true;
        }
        else if (!ActionIndex.HasValue)
        {
            visible = false;
            loadedForIndex = null;
            local = null;
        }
    }
    private void AddSelector()
    {
        if (local == null) return;
        local.ExtractRules.Add(new ExtractRuleDto());
    }
    private void RemoveSelector(int index)
    {
        if (local == null) return;
        if (index < 0 || index >= local.ExtractRules.Count) return;
        local.ExtractRules.RemoveAt(index);
    }
    private async Task OnTestClicked()
    {
        if (local == null || isTesting) return;
        if (string.IsNullOrWhiteSpace(PageUrl))
        {
            errorMessage = "Page URL is empty";
            return;
        }
        isTesting = true;
        errorMessage = null;
        _testCts?.Cancel();
        _testCts = new CancellationTokenSource();
        try
        {
            var html = await PlaywrightService.GetHtmlAsync(PageUrl, _testCts.Token);
            if (string.IsNullOrEmpty(html))
            {
                errorMessage = "Failed to load HTML";
                return;
            }
            var selectors = local.ExtractRules.Select(r => r.Value).ToList();
            var elements = ParsingService.Extract(html, selectors);
            previewValues = elements
                .Select(e => e.InnerText)
                .Where(t => !string.IsNullOrEmpty(t))
                .ToList();
            if (elements.Count > 0)
            {
                rawHtml = string.Join("\n\n---\n\n", elements.Select(e => e.OuterHtml));
            }
            else
            {
                rawHtml = html;
            }
        }
        catch (OperationCanceledException)
        {
            return;
        }
        catch (Exception ex)
        {
            errorMessage = $"Test failed: {ex.Message}" +
                           (ex.InnerException is null ? "" : $" | {ex.InnerException.Message}");
        }
        finally
        {
            isTesting = false;
        }
    }
    private async Task OnSaveClicked()
    {
        if (local == null) return;
        await OnApplied.InvokeAsync(local);
        await CloseAsync();
    }
    private async Task OnCancelClicked()
    {
        await CloseAsync();
    }
    private async Task CloseAsync()
    {
        _testCts?.Cancel();
        _testCts?.Dispose();
        _testCts = null;
        visible = false;
        local = null;
        rawHtml = "";
        previewValues = new();
        loadedForIndex = null;
        await OnClose.InvokeAsync();
    }
}
