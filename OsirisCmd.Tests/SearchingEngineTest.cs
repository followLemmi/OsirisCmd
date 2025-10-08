using Application;
using Xunit.Abstractions;
using System.Reflection;
using Application.Services.FileSearcher;
using Avalonia.Controls.Shapes;
using Path = System.IO.Path;

namespace OsirisCmd.Tests;

public class SearchingEngineTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public SearchingEngineTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void ContentEntriesTest()
    {
        var assemblyLocation = Assembly.GetExecutingAssembly().Location;
        var assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
        var testDataPath = Path.Combine(assemblyDirectory, "TestData", "test_book.txt");
        
        var fileInfo = new FileInfo(testDataPath);
        var entries = SEContentUtils.ParseResultEntries(fileInfo.FullName, "*потому что*", false);
        _testOutputHelper.WriteLine(entries.Count.ToString());
        foreach (var entry in entries)
        {
            _testOutputHelper.WriteLine(entry.Key + " - " + entry.Value);
        }
    }

    // [Fact]
    // public void LineEntriesTest() {
    //     const string testText = "Test text for search in line Test";
    //     const string contentToSearch = "Test";
    //     var match1 = new LineMatchResult
    //     {
    //         StartIndex = 0,
    //         EndIndex = contentToSearch.Length,
    //         Text = contentToSearch,
    //     };
    //     var match2 = new LineMatchResult
    //     {
    //         StartIndex = 29,
    //         EndIndex = 29 + contentToSearch.Length,
    //         Text = contentToSearch,
    //     };
    //     
    //     // Exact match test
    //     var result = SEContentUtils.GetLineEntriesPositions(testText, "Test", false, true);
    //     Assert.Equal(2, result.Count);
    //     Assert.Equal(match1, result[0], LineMatchResult.StartIndexEndIndexTextComparer);
    //     Assert.Equal(match2, result[1], LineMatchResult.StartIndexEndIndexTextComparer);
    //     
    //     // Wildcard match test
    //     result = SEContentUtils.GetLineEntriesPositions(testText, "*Test", true, false);
    //     Assert.Equal(2, result.Count);
    //     Assert.Equal(match1, result[0], LineMatchResult.StartIndexEndIndexTextComparer);
    //     Assert.Equal(match2, result[1], LineMatchResult.StartIndexEndIndexTextComparer);
    //     
    //     result = SEContentUtils.GetLineEntriesPositions(testText, "Test*", true, false);
    //     Assert.Equal(2, result.Count);
    //     Assert.Equal(match1, result[0], LineMatchResult.StartIndexEndIndexTextComparer);
    //     Assert.Equal(match2, result[1], LineMatchResult.StartIndexEndIndexTextComparer);
    //
    //     const string resultWord3 = "text";
    //     var match3 = new LineMatchResult
    //     {
    //         StartIndex = 5,
    //         EndIndex = 5 + resultWord3.Length,
    //         Text = resultWord3,
    //     };
    //     result = SEContentUtils.GetLineEntriesPositions(testText, "Te*t", true, false);
    //     Assert.Equal(3, result.Count);
    //     Assert.Equal(match1, result[0], LineMatchResult.StartIndexEndIndexTextComparer);
    //     Assert.Equal(match3, result[1], LineMatchResult.StartIndexEndIndexTextComparer);
    //     Assert.Equal(match2, result[2], LineMatchResult.StartIndexEndIndexTextComparer);
    //     
    //     result = SEContentUtils.GetLineEntriesPositions(testText, "Te*t", true, true);
    //     Assert.Equal(2, result.Count);
    //     Assert.Equal(match1, result[0], LineMatchResult.StartIndexEndIndexTextComparer);
    //     Assert.Equal(match2, result[1], LineMatchResult.StartIndexEndIndexTextComparer);
    //     
    //     // Two words test
    //     result = SEContentUtils.GetLineEntriesPositions(testText, "Test text", true, false);
    //     const string twoWordsContentToSearch = "Test text";
    //     var twoWordsMatch = new LineMatchResult
    //     {
    //         StartIndex = 0,
    //         EndIndex = twoWordsContentToSearch.Length,
    //         Text = twoWordsContentToSearch,
    //     };
    //     Assert.Single(result);
    //     Assert.Equal(twoWordsMatch, result[0], LineMatchResult.StartIndexEndIndexTextComparer);
    // }
}
