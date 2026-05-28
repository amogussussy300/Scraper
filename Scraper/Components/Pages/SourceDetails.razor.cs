using Microsoft.AspNetCore.Components;
using Scraper.Core.Dtos;
using Scraper.Core.Services;
namespace Scraper.Components.Pages;

public partial class SourceDetails
{
    [Parameter] public int Id { get; set; }
    [Inject] private SourceService SourceService { get; set; } = default!;
    [Inject] private ScrapDetailService ScrapDetailService { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    private SourceDto? source;
    private List<ScrapDetailDto> details = new();
    private bool isCreating;
    // Какой detail сейчас открыт в overlay (null = overlay закрыт).
    // Используется в шаге E.
    private int? editingDetailId;
    protected override async Task OnInitializedAsync()
    {
        await LoadAsync();
    }
    private async Task LoadAsync()
    {
        source = await SourceService.GetByIdAsync(Id);
        if (source == null)
        {
            details = new();
            return;
        }
        details = await ScrapDetailService.GetBySourceAsync(Id);
    }
    private async Task OnCreateDetailClicked()
    {
        if (isCreating || source == null) return;
        isCreating = true;
        try
        {
            var created = await ScrapDetailService.CreateAsync(Id);
            details.Add(created);
        }
        finally
        {
            isCreating = false;
        }
    }
    private void OnEditClicked(int detailId)
    {
        editingDetailId = detailId;
    }

    private void OnOverlayClosed()
    {
        editingDetailId = null;
    }

    private async Task OnOverlaySaved()
    {
        // Перезагружаем таблицу — Target мог измениться,
        // и пользователь должен увидеть новое значение.
        details = await ScrapDetailService.GetBySourceAsync(Id);
    }
}