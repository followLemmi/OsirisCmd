using OsirisCmd.SearchingEngine;
using Xunit.Abstractions;

namespace OsirisCmd.Tests;

public class UnitTest1
{
    private readonly ITestOutputHelper _testOutputHelper;

    public UnitTest1(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void Test1()
    {
        var startTimestamp = DateTime.Now;
        var fileSearcher = new FileSearcher("./indexes");
        var result = fileSearcher.SearchByFileContent("Aft");
        // var result = fileSearcher.SearchByFileName("Test");
        foreach (var searchResult in result)
        {
            _testOutputHelper.WriteLine(searchResult.ToString());
        }
        var endTimestamp = DateTime.Now;
        _testOutputHelper.WriteLine($"Time elapsed: {(endTimestamp - startTimestamp).TotalSeconds} ms");
    }
}
