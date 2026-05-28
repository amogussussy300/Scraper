using Microsoft.AspNetCore.Components;
using Scraper.Core.Dtos;
using Scraper.Core.Services;

namespace Scraper.Components;

public partial class EditDetailOverlay
{
    [Parameter] public int? DetailId { get; set; }
    [Parameter] public string PageUrl { get; set; } = "";
    [Parameter] public int? SourceId { get; set; }
    [Parameter] public EventCallback OnClose { get; set; }
    [Parameter] public EventCallback OnSaved { get; set; }

    [Inject] private ScrapDetailService ScrapDetailService { get; set; } = default!;
    [Inject] private IPlaywrightService PlaywrightService { get; set; } = default!;
    [Inject] private HtmlParserService HtmlParserService { get; set; } = default!;

    private ScrapDetailDto? detail;
    private string rawHtml = "";
    private List<string> previewValues = new();
    private bool isTesting;
    private bool isSaving;
    private string? errorMessage;
    private bool visible;
    private int? loadedForId;

    private const int HtmlPreviewLimit = 50_000;

    private string TruncatedHtml =>
        rawHtml.Length > HtmlPreviewLimit
            ? rawHtml.Substring(0, HtmlPreviewLimit) + $"\n\n... [truncated, total {rawHtml.Length} chars]"
            : rawHtml;

    protected override async Task OnParametersSetAsync()
    {
        if (DetailId.HasValue && DetailId != loadedForId)
        {
            await LoadAsync(DetailId.Value);
            loadedForId = DetailId;
            visible = true;
        }
        else if (!DetailId.HasValue)
        {
            visible = false;
            loadedForId = null;
        }
    }

    private async Task LoadAsync(int id)
    {
        errorMessage = null;
        rawHtml = "";
        previewValues = new();
        detail = await ScrapDetailService.GetWithActionsAsync(id);
        if (detail == null)
        {
            errorMessage = "Detail not found";
        }
    }

    private void AddSelector()
    {
        if (detail == null) return;
        detail.Actions.Add(new ScrapActionDto
        {
            ScrapDetailId = detail.Id,
            Selector = "",
            Order = detail.Actions.Count
        });
    }

    private void RemoveSelector(int index)
    {
        if (detail == null) return;
        if (index < 0 || index >= detail.Actions.Count) return;
        detail.Actions.RemoveAt(index);
    }

    private async Task OnTestClicked()
    {
        if (detail == null || isTesting) return;
        if (string.IsNullOrWhiteSpace(PageUrl))
        {
            errorMessage = "Page URL is empty (set Source.Link)";
            return;
        }

        isTesting = true;
        errorMessage = null;
        try
        {
            var cacheKey = $"source-{SourceId}";
            var html = await PlaywrightService.GetHtmlAsync(PageUrl, cacheKey);
            if (string.IsNullOrEmpty(html))
            {
                errorMessage = "Failed to load HTML";
                return;
            }

            var actions = detail.Actions
                .Select((a, i) => new Scraper.Core.Entities.ScrapAction
                {
                    Selector = a.Selector,
                    Order = i
                })
                .ToList();

            var result = HtmlParserService.Parse(html, actions);
            previewValues = result.Values;

            if (result.MatchedHtml.Count > 0)
            {
                rawHtml = string.Join("\n\n---\n\n", result.MatchedHtml);
            }
            else
            {
                rawHtml = html;
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"Test failed: {ex.Message}";
        }
        finally
        {
            isTesting = false;
        }
    }

    private async Task OnSaveClicked()
    {
        if (detail == null || isSaving) return;
        isSaving = true;
        errorMessage = null;
        try
        {
            await ScrapDetailService.SaveAsync(detail.Id, detail.Target, detail.Actions);
            await OnSaved.InvokeAsync();
            await CloseAsync();
        }
        catch (Exception ex)
        {
            errorMessage = $"Save failed: {ex.Message}";
        }
        finally
        {
            isSaving = false;
        }
    }

    private async Task OnCancelClicked()
    {
        await CloseAsync();
    }

    private async Task CloseAsync()
    {
        visible = false;
        detail = null;
        rawHtml = "";
        previewValues = new();
        loadedForId = null;
        await OnClose.InvokeAsync();
    }
}
