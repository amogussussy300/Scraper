using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Scraper.Core;
using Scraper.Core.Dtos;
using Scraper.Core.Enums;
using Scraper.Parsing;
using Scraper.Parsing.Services;
using Scraper.Services;

namespace Scraper.Components.Pages;

public partial class EditPageOverlay : ComponentBase
{
    [Parameter] public int PageId { get; set; }
    [Parameter] public EventCallback OnSaved { get; set; }

    [Inject] private PageStore Store { get; set; } = default!;
    [Inject] private IPlaywrightService PlaywrightService { get; set; } = default!;
    [Inject] private ParsingService ParsingService { get; set; } = default!;

    private PageDto? working;

    private int? selectedActionIndex;
    private ActionDto? selectedAction;
    private bool isActionOverlayOpen;

    private ActionType newActionType = ActionType.ExtractField;
    private string newActionFieldName = "";

    private bool isPreviewOpen;
    private bool isPreviewLoading;
    private string? previewError;
    private Dictionary<string, List<string>> previewResults = new();

    private IEnumerable<string> AvailableFields =>
        TempFields.All.Where(f => working?.Actions.All(a => a.FieldName != f) == true);

    protected override void OnParametersSet()
    {
        if (PageId != 0)
        {
            var orig = Store.Get(PageId);
            if (orig != null)
            {
                var json = JsonSerializer.Serialize(orig);
                working = JsonSerializer.Deserialize<PageDto>(json);
            }
            else
            {
                working = null;
            }
        }
        else
        {
            working = null;
        }

        if (!string.IsNullOrEmpty(newActionFieldName)
            && working?.Actions.Any(a => a.FieldName == newActionFieldName) == true)
        {
            newActionFieldName = "";
        }
    }

    private void ConfirmCreateAction()
    {
        if (working == null || string.IsNullOrEmpty(newActionFieldName)) return;
        if (working.Actions.Any(a => a.FieldName == newActionFieldName)) return;
        working.Actions.Add(new ActionDto
        {
            ActionType = newActionType,
            FieldName = newActionFieldName
        });
        newActionFieldName = "";
    }

    private void OnEditActionClicked(int idx)
    {
        if (working == null || idx < 0 || idx >= working.Actions.Count) return;
        var json = JsonSerializer.Serialize(working.Actions[idx]);
        selectedAction = JsonSerializer.Deserialize<ActionDto>(json);
        selectedActionIndex = idx;
        isActionOverlayOpen = true;
    }

    private void OnDeleteActionClicked(int idx)
    {
        if (working == null || idx < 0 || idx >= working.Actions.Count) return;
        working.Actions.RemoveAt(idx);
    }

    private void OnActionOverlayClosed()
    {
        selectedActionIndex = null;
        selectedAction = null;
        isActionOverlayOpen = false;
    }

    private void OnActionApplied(ActionDto updated)
    {
        if (working != null && selectedActionIndex.HasValue
            && selectedActionIndex.Value < working.Actions.Count)
        {
            working.Actions[selectedActionIndex.Value] = updated;
        }
        selectedActionIndex = null;
        selectedAction = null;
        isActionOverlayOpen = false;
    }

    private void OnReloadActions()
    {
        if (working == null || string.IsNullOrWhiteSpace(working.Url)) return;
        Store.Update(PageId, working);
    }

    private async Task OnSaveClicked()
    {
        if (working == null) return;
        if (string.IsNullOrWhiteSpace(working.Url)) return;
        Store.Update(PageId, working);
        await OnSaved.InvokeAsync();
    }

    private async Task OpenPreview()
    {
        isPreviewOpen = true;
        await RefreshPreview();
    }

    private void ClosePreview()
    {
        isPreviewOpen = false;
        previewResults.Clear();
        previewError = null;
    }

    private async Task RefreshPreview()
    {
        if (working == null || string.IsNullOrWhiteSpace(working.Url)) return;
        isPreviewLoading = true;
        previewError = null;
        previewResults.Clear();
        try
        {
            var html = await PlaywrightService.GetHtmlAsync(working.Url);
            if (string.IsNullOrEmpty(html))
            {
                previewError = "Failed to load HTML from URL";
                return;
            }

            for (int i = 0; i < working.Actions.Count; i++)
            {
                var action = working.Actions[i];
                if (string.IsNullOrEmpty(action.FieldName)) continue;
                var selectors = action.ExtractRules
                    .Select(r => r.Value)
                    .Where(v => !string.IsNullOrWhiteSpace(v))
                    .ToList();
                if (selectors.Count == 0) continue;

                var elements = ParsingService.Extract(html, selectors);
                var values = elements
                    .Select(e => e.InnerText)
                    .Where(v => !string.IsNullOrEmpty(v))
                    .ToList();
                if (values.Count > 0 && !previewResults.ContainsKey(action.FieldName))
                    previewResults[action.FieldName] = values;
            }
        }
        catch (Exception ex)
        {
            previewError = $"Preview failed: {ex.Message}";
        }
        finally
        {
            isPreviewLoading = false;
        }
    }
}
