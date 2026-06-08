using Microsoft.AspNetCore.Components;
using Scraper.Core.Dtos;
using Scraper.Core.Services;
namespace Scraper.Components.Pages;

public partial class SourceDetail
{
    [Parameter] public int Id { get; set; }
    [Inject] private SourceService SourceService { get; set; } = default!;
    private SourceDto? source;
    protected override async Task OnInitializedAsync()
    {
        source = await SourceService.GetByIdAsync(Id);
    }
}
