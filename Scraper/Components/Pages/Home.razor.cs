using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using Scraper.Data;
using Scraper.Services;
namespace Scraper.Components.Pages;

public partial class Home
{
    [Inject] private ItemService ItemService { get; set; } = default!;
    private List<ItemEntity> items = new();
    private bool isModalOpen;
    private bool isSaving;
    private string? saveError;
    private NewItemForm model = new();
    protected override async Task OnInitializedAsync()
    {
        items = await ItemService.GetAllAsync();
    }
    private void OnAddClicked()
    {
        model = new NewItemForm();
        saveError = null;
        isModalOpen = true;
    }
    private void CloseModal()
    {
        isModalOpen = false;
        saveError = null;
        model = new NewItemForm();
    }
    private async Task SaveItem()
    {
        if (isSaving) return;
        isSaving = true;
        try
        {
            var trimmed = (model.Link ?? "").Trim();
            bool hasScheme = Uri.TryCreate(trimmed, UriKind.Absolute, out var probe)
                             && (probe.Scheme == Uri.UriSchemeHttp || probe.Scheme == Uri.UriSchemeHttps);
            model.Link = hasScheme ? trimmed : "https://" + trimmed;
            var saved = await ItemService.AddAsync(model.Name, model.Link);
            if (saved is null)
            {
                saveError = "An item with this link already exists.";
                return;
            }
            items.Add(saved);
            CloseModal();
        }
        finally
        {
            isSaving = false;
        }
    }
}
public class NewItemForm
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; } = "";
    [Required(ErrorMessage = "Link is required")]
    [ValidUrl(ErrorMessage = "Link must be a valid http(s) URL with a real domain")]
    public string Link { get; set; } = "";
}
public sealed class ValidUrlAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string s || string.IsNullOrWhiteSpace(s))
            return ValidationResult.Success;
        var trimmed = s.Trim();
        var candidate = trimmed.Contains("://", StringComparison.Ordinal) ? trimmed : "https://" + trimmed;
        if (!Uri.TryCreate(candidate, UriKind.Absolute, out var uri)
            || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
            || Uri.CheckHostName(uri.Host) != UriHostNameType.Dns)
            return new ValidationResult(ErrorMessage);
        int dot = uri.Host.LastIndexOf('.');
        if (dot <= 0 || dot == uri.Host.Length - 1) return new ValidationResult(ErrorMessage);
        var tld = uri.Host.AsSpan(dot + 1);
        if (tld.Length < 2) return new ValidationResult(ErrorMessage);
        foreach (var c in tld)
            if (!char.IsAsciiLetter(c)) return new ValidationResult(ErrorMessage);
        return ValidationResult.Success;
    }
}