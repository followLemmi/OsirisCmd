using Application;
using Xunit.Abstractions;
using System.Reflection;

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
        // Получаем директорию, где находится сборка тестов
        var assemblyLocation = Assembly.GetExecutingAssembly().Location;
        var assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
        var testDataPath = Path.Combine(assemblyDirectory, "TestData", "test_book.txt");
        
        var fileInfo = new FileInfo(testDataPath);
        var entries = SEContentUtils.parseResultEntries(fileInfo.FullName, "*потому что*", false);
        _testOutputHelper.WriteLine(entries.Count.ToString());
        foreach (var entry in entries)
        {
            _testOutputHelper.WriteLine(entry.Key + " - " + entry.Value);
        }
    }
}