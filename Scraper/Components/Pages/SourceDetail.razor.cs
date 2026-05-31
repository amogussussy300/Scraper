using Microsoft.AspNetCore.Components;
using Scraper.Core.Dtos;
using Scraper.Core.Services;
namespace Scraper.Components.Pages;

public partial class SourceDetail
{
    [Parameter] public int Id { get; set; }
    [Inject] private SourceService SourceService { get; set; } = default!;
    private SourceDto? source;
    private int? selectedPageId;
    protected override async Task OnInitializedAsync()
    {
        source = await SourceService.GetByIdAsync(Id);
    }
    private void OnPageSelected(int pageId)
    {
        selectedPageId = pageId;
    }
    private void OnPageSaved()
    {
        selectedPageId = null;
    }
}