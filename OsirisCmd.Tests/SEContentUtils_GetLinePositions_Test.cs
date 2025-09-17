using System.Text.RegularExpressions;
using Application.Services.FileSearcher;

namespace OsirisCmd.Tests;

public class SEContentUtils_GetLinePositions_Test
{
    private const string Content = "Test text for search in line Test test";

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
    
}