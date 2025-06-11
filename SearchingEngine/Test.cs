namespace SearchingEngine;

public class Test
{
    public static void Main()
    {
        var startTimestamp = DateTime.Now;
        var fileSearcher = new FileSearcher("./indexes");
        // var result = fileSearcher.SearchByFileContent("Printer");
        var result = fileSearcher.SearchByFileName("battle.*");
        foreach (var searchResult in result)
        {
            Console.WriteLine(searchResult);
        }
        var endTimestamp = DateTime.Now;
        Console.WriteLine($"Time elapsed: {(endTimestamp - startTimestamp).TotalSeconds} ms");
    }
}