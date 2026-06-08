namespace Scraper.Core;

public static class TempFields
{
    public const string Title = "Title";
    public const string Price = "Price";
    public const string Description = "Description";
    public const string ImageUrl = "ImageUrl";
    public const string Misc = "Misc";
    public static IReadOnlyList<string> All { get; } = new[]
    {
        Title, Price, Description, ImageUrl, Misc
    };
}