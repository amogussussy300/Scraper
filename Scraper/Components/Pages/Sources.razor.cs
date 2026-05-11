using Microsoft.AspNetCore.Components;
using Scraper.Core.Dtos;
using Scraper.Core.Services;
using Scraper.Models;
namespace Scraper.Components.Pages;

public partial class Sources : IDisposable
{
    [Inject] private SourceService SourceService { get; set; } = default!;

    private List<SourceDto> sources = new();
    private bool isModalOpen;
    private bool isSaving;
    private SourceModel model = new();

    private int currentPage = 1;
    private int totalPages = 1;
    private const int PageSize = 10;

    private string sortColumn = "Name";
    private bool sortDescending;

    private string nameFilter = "";
    private string linkFilter = "";

    private CancellationTokenSource? _debounce;
    private const int DebounceMs = 300;

    protected override async Task OnInitializedAsync()
    {
        await FetchDataAsync();
    }

    private async Task FetchDataAsync()
    {
        currentPage = 1;
        var result = await SourceService.GetFilteredAsync(
            page: currentPage,
            pageSize: PageSize,
            nameFilter: string.IsNullOrWhiteSpace(nameFilter) ? null : nameFilter,
            linkFilter: string.IsNullOrWhiteSpace(linkFilter) ? null : linkFilter,
            sortBy: sortColumn,
            sortDescending: sortDescending);

        sources = result.Items;
        totalPages = result.TotalPages;
        currentPage = result.Page;

    }

    private async Task OnSortClicked(string column)
    {
        if (sortColumn == column)
        {
            sortDescending = !sortDescending;
        }
        else
        {
            sortColumn = column;
            sortDescending = false;
        }
        await FetchDataAsync();
    }

    private string SortIcon(string column)
    {
        if (sortColumn != column) return "";
        return sortDescending ? "bi-arrow-down" : "bi-arrow-up";
    }

    private async Task OnNameSearchChanged(ChangeEventArgs e)
    {
        nameFilter = e.Value?.ToString() ?? "";
        await DebouncedFetchAsync();
    }

    private async Task OnLinkSearchChanged(ChangeEventArgs e)
    {
        linkFilter = e.Value?.ToString() ?? "";
        await DebouncedFetchAsync();
    }

    private async Task GoToPage(int page)
    {
        if (page < 1 || page > totalPages) return;
        currentPage = page;
        var result = await SourceService.GetFilteredAsync(page: currentPage, pageSize: PageSize,
            nameFilter: string.IsNullOrWhiteSpace(nameFilter) ? null : nameFilter,
            linkFilter: string.IsNullOrWhiteSpace(linkFilter) ? null : linkFilter,
            sortBy: sortColumn,
            sortDescending: sortDescending);
        sources = result.Items;
        totalPages = result.TotalPages;
        currentPage = result.Page;
    }

    private async Task DebouncedFetchAsync()
    {
        _debounce?.Cancel();
        _debounce = new CancellationTokenSource();
        try
        {
            await Task.Delay(DebounceMs, _debounce.Token);
        }
        catch (TaskCanceledException)
        {
            return;
        }
        await FetchDataAsync();
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
            await FetchDataAsync();
            CloseModal();
        }
        finally
        {
            isSaving = false;
        }
    }

    public void Dispose()
    {
        _debounce?.Cancel();
        _debounce?.Dispose();
    }
}
