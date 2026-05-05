using Microsoft.AspNetCore.Components;
using Scraper.Core.Dtos;
using Scraper.Core.Services;

namespace Scraper.Components.Pages;

public partial class Sources
{
    [Inject] private SourceService SourceService { get; set; } = default!;

    private List<SourceDto> sources = new();
    private bool isModalOpen;
    private bool isSaving;
    private SourceDto model = new();

    protected override async Task OnInitializedAsync()
    {
        sources = await SourceService.GetAllAsync();
    }

    private void OnAddClicked()
    {
        model = new SourceDto();
        isModalOpen = true;
    }

    private void CloseModal()
    {
        isModalOpen = false;
        model = new SourceDto();
    }

    private async Task SaveSource()
    {
        if (isSaving) return;
        isSaving = true;
        try
        {
            var trimmed = (model.Link ?? "").Trim();
            bool hasScheme = Uri.TryCreate(trimmed, UriKind.Absolute, out var probe)
                             && (probe.Scheme == Uri.UriSchemeHttp || probe.Scheme == Uri.UriSchemeHttps);
            model.Link = hasScheme ? trimmed : "https://" + trimmed;

            var saved = await SourceService.AddAsync(model);
            sources.Add(saved);
            CloseModal();
        }
        finally
        {
            isSaving = false;
        }
    }
}
