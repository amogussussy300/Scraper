using Microsoft.AspNetCore.Components;
using Scraper.Core.Dtos;
using Scraper.Core.Services;
using Scraper.Models;

namespace Scraper.Components.Pages;

public partial class Sources
{
    [Inject] private SourceService SourceService { get; set; } = default!;

    private List<SourceDto> sources = new();
    private bool isModalOpen;
    private bool isSaving;
    private SourceModel model = new();

    protected override async Task OnInitializedAsync()
    {
        sources = await SourceService.GetAllAsync();
    }

    private void OnAddClicked()
    {
        model = new SourceModel();
        isModalOpen = true;
    }

    private void CloseModal()
    {
        isModalOpen = false;
        model = new SourceModel();
    }

    private async Task SaveSource()
    {
        if (isSaving) return;
        isSaving = true;
        try
        {
            var dto = new SourceDto { Name = model.Name, Link = model.Link };
            var saved = await SourceService.AddAsync(dto);
            sources.Add(saved);
            CloseModal();
        }
        finally
        {
            isSaving = false;
        }
    }
}
