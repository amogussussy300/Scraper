using Microsoft.AspNetCore.Components;
using Scraper.Core.Dtos;
using Scraper.Services;
namespace Scraper.Components.Pages;

public partial class PagesList : ComponentBase
{
    [Parameter] public int SourceId { get; set; }
    [Inject] private PageStore Store { get; set; } = default!;
    private List<(int pid, PageDto page)> pages = new();
    private bool isCreateOpen;
    private string newName = "";
    private string newUrl = "";
    private int? expandedPageId;
    protected override void OnParametersSet()
    {
        Reload();
    }
    private void Reload() => pages = Store.GetWithIdsForSource(SourceId).ToList();
    private void OpenCreateModal()
    {
        newName = "";
        newUrl = "";
        isCreateOpen = true;
    }
    private void CloseCreateModal() => isCreateOpen = false;
    private void ConfirmCreate()
    {
        if (string.IsNullOrWhiteSpace(newName) || string.IsNullOrWhiteSpace(newUrl)) return;
        var url = newUrl.Trim();
        if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) && !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            url = "https://" + url;
        Store.Create(SourceId, newName.Trim(), url);
        isCreateOpen = false;
        Reload();
    }
    private void DeletePage(int pid)
    {
        Store.Delete(pid);
        if (expandedPageId == pid) expandedPageId = null;
        Reload();
    }
    private void ToggleAccordion(int pid)
    {
        expandedPageId = expandedPageId == pid ? null : pid;
    }
    private void OnPageSaved()
    {
        expandedPageId = null;
        Reload();
    }
}
