using System.Reflection;
using Application.Services.FileSearcher;

namespace OsirisCmd.Tests;

public class SEContentUtils_ParseResultEntries_Test
{
    private static string GetTestBookPath()
    {
        var assemblyLocation = Assembly.GetExecutingAssembly().Location;
        var assemblyDirectory = Path.GetDirectoryName(assemblyLocation)!;
        return Path.Combine(assemblyDirectory, "TestData", "test_book.txt");
    }

    [Fact]
    public void SingleWord_CaseSensitive_Annotation_Test()
    {
        var filePath = GetTestBookPath();
        var entries = SEContentUtils.ParseResultEntries(filePath, "Annotation", true);

        Assert.Single(entries);
        var (lineIndex, matches) = entries.Single();

        var fileLines = File.ReadAllLines(filePath);
        Assert.Equal("Annotation", fileLines[lineIndex]);

        var expected = new List<LineMatchResult>
        {
            new() { StartIndex = 0, EndIndex = "Annotation".Length, Text = "Annotation" }
        };
        Assert.Equal(expected, matches, LineMatchResult.StartIndexEndIndexTextComparer);
    }

    [Fact]
    public void SingleWord_CaseInsensitive_Annotation_Test()
    {
        var filePath = GetTestBookPath();
        var entries = SEContentUtils.ParseResultEntries(filePath, "annotation", false);

        Assert.Single(entries);
        var (lineIndex, matches) = entries.Single();

        var fileLines = File.ReadAllLines(filePath);
        Assert.Equal("Annotation", fileLines[lineIndex]);

        var expected = new List<LineMatchResult>
        {
            new() { StartIndex = 0, EndIndex = "annotation".Length, Text = "Annotation" }
        };
        Assert.Equal(expected, matches, LineMatchResult.StartIndexEndIndexTextComparer);
    }

    [Fact]
    public void PhraseSearch_Quoted_AsterisksLine_Test()
    {
        var filePath = GetTestBookPath();
        var entries = SEContentUtils.ParseResultEntries(filePath, "\"* * *\"", true);

        // The file contains many separator lines "* * *"; ensure there are multiple matches.
        Assert.True(entries.Count >= 1);

        var fileLines = File.ReadAllLines(filePath);
        foreach (var kvp in entries)
        {
            var lineIndex = kvp.Key;
            var matches = kvp.Value;
            Assert.Equal("* * *", fileLines[lineIndex]);

            var expected = new List<LineMatchResult>
            {
                new() { StartIndex = 0, EndIndex = "* * *".Length, Text = "* * *" }
            };
            Assert.Equal(expected, matches, LineMatchResult.StartIndexEndIndexTextComparer);
        }
    }

    [Fact]
    public void MultiWord_Unquoted_FullPhraseOnLine_Test()
    {
        var filePath = GetTestBookPath();
        var entries = SEContentUtils.ParseResultEntries(filePath, "Предисловие переводчика", true);

        Assert.True(entries.Count >= 1);

        var fileLines = File.ReadAllLines(filePath);
        foreach (var kvp in entries)
        {
            var lineIndex = kvp.Key;
            var matches = kvp.Value;
            Assert.Equal("Предисловие переводчика", fileLines[lineIndex]);

            var expected = new List<LineMatchResult>
            {
                new() { StartIndex = 0, EndIndex = "Предисловие переводчика".Length, Text = "Предисловие переводчика" }
            };
            Assert.Equal(expected, matches, LineMatchResult.StartIndexEndIndexTextComparer);
        }
    }
}