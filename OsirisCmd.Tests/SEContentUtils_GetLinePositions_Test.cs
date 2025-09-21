using Application.Services.FileSearcher;

namespace OsirisCmd.Tests;

public class SEContentUtils_GetLinePositions_Test
{
    private const string Content = "Test text for search in line Test test LineSearching linesearching";

    [Fact]
    public void ExactWord_CaseSensitive_Test()
    {
        const string searchingContent = "Test";
        var expectedMatches = new List<LineMatchResult>
        {
            new() {StartIndex = 0, EndIndex = searchingContent.Length, Text = "Test"},
            new() {StartIndex = 29, EndIndex = 29 + searchingContent.Length, Text = "Test"},
        };
        var result = SEContentUtils.GetLineEntriesPositions(Content, searchingContent, false, true);
        Assert.Equal(expectedMatches, result, LineMatchResult.StartIndexEndIndexTextComparer);
    }
    
    [Fact]
    public void ExactWord_CaseInsensitive_Test()
    {
        const string searchingContent = "Test";
        var expectedMatches = new List<LineMatchResult>
        {
            new() {StartIndex = 0, EndIndex = searchingContent.Length, Text = "Test"},
            new() {StartIndex = 29, EndIndex = 29 + searchingContent.Length, Text = "Test"},
            new() {StartIndex = 34, EndIndex = 34 + searchingContent.Length, Text = "test"},
        };
        var result = SEContentUtils.GetLineEntriesPositions(Content, searchingContent, false, false);
        Assert.Equal(expectedMatches, result, LineMatchResult.StartIndexEndIndexTextComparer);
    }

    [Theory]
    [InlineData("*Test*")]
    [InlineData("*Test")]
    [InlineData("Test*")]
    public void Wildcard_SimplePattern_CaseSensitive_Test(string searchingText)
    {
        var exactWord = new string(searchingText.Where(char.IsLetterOrDigit).ToArray());
        var expectedMatches = new List<LineMatchResult>
        {
            new() {StartIndex = 0, EndIndex = exactWord.Length, Text = "Test"},
            new() {StartIndex = 29, EndIndex = 29 + exactWord.Length, Text = "Test"},
        };
        var result = SEContentUtils.GetLineEntriesPositions(Content, searchingText, true, true);
        Assert.Equal(expectedMatches, result, LineMatchResult.StartIndexEndIndexTextComparer);
    }

    [Theory]
    [InlineData("*Test*")]
    [InlineData("*Test")]
    [InlineData("Test*")]
    [InlineData("*test*")]
    [InlineData("*test")]
    [InlineData("test*")]
    public void Wildcard_SimplePattern_CaseInsensitive_Test(string searchingText)
    {
        var exactWord = new string(searchingText.Where(char.IsLetterOrDigit).ToArray());
        var expectedMatches = new List<LineMatchResult>
        {
            new() {StartIndex = 0, EndIndex = exactWord.Length, Text = "Test"},
            new() {StartIndex = 29, EndIndex = 29 + exactWord.Length, Text = "Test"},
            new() {StartIndex = 34, EndIndex = 34 + exactWord.Length, Text = "test"},
        };
        var result = SEContentUtils.GetLineEntriesPositions(Content, searchingText, true, false);
        Assert.Equal(expectedMatches, result, LineMatchResult.StartIndexEndIndexTextComparer);
    }

    [Theory]
    [InlineData("Te*t")]
    public void Wildcard_DifficultPattern_CaseSensitive_Test(string searchingText)
    {
        var expectedMatchLength = 4;
        var expectedMatches = new List<LineMatchResult> {
            new() {StartIndex = 0, EndIndex = expectedMatchLength, Text = "Test"},
            new() {StartIndex = 29, EndIndex = 29 + expectedMatchLength, Text = "Test"}
        };
        var result = SEContentUtils.GetLineEntriesPositions(Content, searchingText, true, true);
        Assert.Equal(expectedMatches, result, LineMatchResult.StartIndexEndIndexTextComparer);
    }

    [Theory]
    [InlineData("te*t")]
    public void Wildcard_DifficultPattern_CaseSensitive_LowerCase_Test(string searchingText)
    {
        var expectedMatchLength = 4;
        var expectedMatches = new List<LineMatchResult> {
            new() {StartIndex = 5, EndIndex = 5 + expectedMatchLength, Text = "text"},
            new() {StartIndex = 34, EndIndex = 34 + expectedMatchLength, Text = "test"}
        };
        var result = SEContentUtils.GetLineEntriesPositions(Content, searchingText, true, true);
        Assert.Equal(expectedMatches, result, LineMatchResult.StartIndexEndIndexTextComparer);
    }

    [Theory]
    [InlineData("Te*t")]
    [InlineData("te*t")]
    public void Wildcard_DifficultPattern_CaseInsensitive_Test(string searchingText)
    {
        var expectedMatchLength = 4;
        var expectedMatches = new List<LineMatchResult>
        {
            new() {StartIndex = 0, EndIndex = expectedMatchLength, Text = "Test"},
            new() {StartIndex = 5, EndIndex = 5 + expectedMatchLength, Text = "text"},
            new() {StartIndex = 29, EndIndex = 29 + expectedMatchLength, Text = "Test"},
            new() {StartIndex = 34, EndIndex = 34 + expectedMatchLength, Text = "test"},
        };
        var result = SEContentUtils.GetLineEntriesPositions(Content, searchingText, true, false);
        Assert.Equal(expectedMatches, result, LineMatchResult.StartIndexEndIndexTextComparer);
    }

    [Fact]
    public void ExactWord_CamelCase_CaseSensitive_Test()
    {
        var searchingText = "LineSearching";
        var expectedMatches = new List<LineMatchResult> {
            new() {StartIndex = 39, EndIndex = 39 + searchingText.Length, Text = "LineSearching"},
        };

        var result = SEContentUtils.GetLineEntriesPositions(Content, searchingText, false, true);
        Assert.Equal(expectedMatches, result, LineMatchResult.StartIndexEndIndexTextComparer);
    }
    
    [Theory]
    [InlineData("LineSearching")]
    [InlineData("linesearching")]
    public void ExactWord_CamelCase_CaseInsensitive_Test(string searchingText)
    {
        var expectedMatches = new List<LineMatchResult> {
            new() {StartIndex = 39, EndIndex = 39 + searchingText.Length, Text = "LineSearching"},
            new() {StartIndex = 53, EndIndex = 53 + searchingText.Length, Text = "linesearching"},
        };

        var result = SEContentUtils.GetLineEntriesPositions(Content, searchingText, false, false);
        Assert.Equal(expectedMatches, result, LineMatchResult.StartIndexEndIndexTextComparer);
    }

    [Theory]
    [InlineData("Line*")]
    [InlineData("*searching")]
    public void Wildcard_CamelCase_CaseInsensitive_Test(string searchingText)
    {
        var expectedMatchLength = 13;
        var expectedMatches = new List<LineMatchResult> {
            new() {StartIndex = 39, EndIndex = 39 + expectedMatchLength, Text = "LineSearching"},
            new() {StartIndex = 53, EndIndex = 53 + expectedMatchLength, Text = "linesearching"},
        };

        var result = SEContentUtils.GetLineEntriesPositions(Content, searchingText, true, false);
        Assert.Equal(expectedMatches, result, LineMatchResult.StartIndexEndIndexTextComparer);
    }
}
