using Microsoft.AspNetCore.Components;
using Scraper.Core.Dtos;
using Scraper.Services;
namespace Scraper.Components.Pages;

public partial class PagesList : ComponentBase
{
    [Parameter] public int SourceId { get; set; }
    [Parameter] public EventCallback<int> OnPageSelected { get; set; }
    [Inject] private PageStore Store { get; set; } = default!;
    private List<(int pid, PageDto page)> pages = new();
    private bool isCreateOpen;
    private string newName = "";
    private string newUrl = "";
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
        Store.Create(SourceId, newName.Trim(), newUrl.Trim());
        isCreateOpen = false;
        Reload();
    }
    private void DeletePage(int pid)
    {
        Store.Delete(pid);
        Reload();
    }
}