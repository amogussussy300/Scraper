using System.ComponentModel.DataAnnotations;

namespace Scraper.Core.Validation;

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
