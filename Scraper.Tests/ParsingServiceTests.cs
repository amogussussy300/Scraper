using Xunit;
using Scraper.Parsing;
using Scraper.Parsing.Services;
using Xunit.Abstractions;

namespace Scraper.Tests;

public class ParsingServiceTests
{
    private readonly ParsingService _parser = new();
    private readonly string _html;
    private readonly ITestOutputHelper _output;

    public ParsingServiceTests(ITestOutputHelper output)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Data", "testHtml.html");
        _html = File.ReadAllText(path);
        _output = output;
    }

    [Fact]
    public void Extract_Title_ReturnsMainTitle()
    {
        var results = _parser.Extract(_html, new[] { "h1.size38" });

        _output.WriteLine($"Text: {results[0].InnerText}");

        Assert.Single(results);
        Assert.Equal("auction - near hospital, remainder of 999 years.", results[0].InnerText);
    }

    [Fact]
    public void Extract_Price_ContainsGuidePrice()
    {
        var results = _parser.Extract(_html, new[] { "p.size27" });

        Assert.Single(results);
        Assert.Contains("£108,000", results[0].InnerText);
    }

    [Fact]
    public void Extract_Description_ReturnsAuctionDescription()
    {
        var results = _parser.Extract(_html, new[] { ".contentReadmore p" });

        Assert.Single(results);
        Assert.Contains("AUCTION SALE", results[0].InnerText);
    }

    [Fact]
    public void Extract_ImageUrl_FromMainPropImage()
    {
        var results = _parser.Extract(_html, new[] { ".mainPropImage" });

        Assert.Single(results);
        Assert.Contains("background-image", results[0].Attr("style") ?? "");
    }


    [Fact]
    public void Extract_AllPropertyCards_ReturnsThree()
    {
        var results = _parser.Extract(_html, new[] { ".Grid-cell .propertyBlock" });

        Assert.Equal(3, results.Count);
    }

    [Fact]
    public void Extract_PropertyCards_Titles()
    {
        var results = _parser.Extract(_html, new[] { ".Grid-cell .propertyBlock", "h3" });

        Assert.Equal(3, results.Count);
        Assert.Equal("bishop's waltham", results[0].InnerText);
        Assert.Equal("northwood square, fareham", results[1].InnerText);
        Assert.Equal("queensway, southampton", results[2].InnerText);
    }

    [Fact]
    public void Extract_PropertyCards_Prices()
    {
        var results = _parser.Extract(_html, new[] { ".Grid-cell .propertyBlock", ".propTextHolder p" });

        Assert.Equal(3, results.Count);
        Assert.Contains("£115,000", results[0].InnerText);
        Assert.Contains("£115,000", results[1].InnerText);
        Assert.Contains("£115,000", results[2].InnerText);
    }

    [Fact]
    public void Extract_PropertyCards_Images()
    {
        var results = _parser.Extract(_html, new[] { ".Grid-cell .propertyBlock", ".propImageHolder" });

        Assert.Equal(3, results.Count);
        Assert.Contains("background-image", results[0].Attr("style") ?? "");
    }


    [Fact]
    public void Extract_ChainedSelectors_NarrowsResults()
    {
        var results = _parser.Extract(_html, new[] { ".propertyBlock", "h3" });

        Assert.Equal(3, results.Count);
    }

    [Fact]
    public void Extract_NoMatchingSelector_ReturnsEmpty()
    {
        var results = _parser.Extract(_html, new[] { ".nonexistent-class" });

        Assert.Empty(results);
    }


    [Fact]
    public void Extract_FirstPropertyCard_TitleOnly()
    {
        var results = _parser.Extract(_html, new[] { ".Grid-cell:first-child .propertyBlock", "h3" });

        Assert.Single(results);
        Assert.Equal("bishop's waltham", results[0].InnerText);
    }

    [Fact]
    public void Extract_FirstPropertyCard_PriceOnly()
    {
        var results = _parser.Extract(_html, new[] { ".Grid-cell:first-child .propertyBlock", ".propTextHolder p" });

        Assert.Single(results);
        Assert.Contains("£115,000", results[0].InnerText);
    }

    [Fact]
    public void Extract_FirstPropertyCard_ImageOnly()
    {
        var results = _parser.Extract(_html, new[] { ".Grid-cell:first-child .propertyBlock", ".propImageHolder" });

        Assert.Single(results);
        Assert.Contains("background-image", results[0].Attr("style") ?? "");
    }

    [Fact]
    public void Extract_SecondPropertyCard_TitleOnly()
    {
        var results = _parser.Extract(_html, new[] { ".Grid-cell:nth-child(2) .propertyBlock", "h3" });

        Assert.Single(results);
        Assert.Equal("northwood square, fareham", results[0].InnerText);
    }

    [Fact]
    public void Extract_SecondPropertyCard_PriceOnly()
    {
        var results = _parser.Extract(_html, new[] { ".Grid-cell:nth-child(2) .propertyBlock", ".propTextHolder p" });

        Assert.Single(results);
        Assert.Contains("£115,000", results[0].InnerText);
    }


    [Fact]
    public void Extract_EmptySelector_Skipped()
    {
        var results = _parser.Extract(_html, new[] { "", "   ", "h1.size38" });

        Assert.Single(results);
        Assert.Equal("auction - near hospital, remainder of 999 years.", results[0].InnerText);
    }
}
