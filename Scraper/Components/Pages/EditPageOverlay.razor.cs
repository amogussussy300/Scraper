using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Scraper.Core.Dtos;
using Scraper.Services;

namespace Scraper.Components.Pages;

public partial class EditPageOverlay : ComponentBase
{
    [Parameter] public int PageId { get; set; }
    [Parameter] public EventCallback OnClose { get; set; }
    [Parameter] public EventCallback OnSaved { get; set; }

    [Inject] private PageStore Store { get; set; } = default!;

    private PageDto? working;
    private bool visible;
    private int? loadedForId;

    private int? selectedActionIndex;
    private ActionDto? selectedAction;
    private bool isActionOverlayOpen;

    protected override void OnParametersSet()
    {
        if (PageId != 0 && PageId != loadedForId)
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
            loadedForId = PageId;
            visible = true;
        }
        else if (PageId == 0)
        {
            visible = false;
            loadedForId = null;
            working = null;
        }
    }

    private void OnCreateAction()
    {
        if (working == null) return;
        working.Actions.Add(new ActionDto());
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

    private async Task OnSaveClicked()
    {
        if (working == null) return;
        Store.Update(PageId, working);
        await OnSaved.InvokeAsync();
        await CloseAsync();
    }

    private async Task OnCancelClicked()
    {
        await CloseAsync();
    }

    private async Task CloseAsync()
    {
        visible = false;
        working = null;
        loadedForId = null;
        selectedActionIndex = null;
        selectedAction = null;
        isActionOverlayOpen = false;
        await OnClose.InvokeAsync();
    }
}
