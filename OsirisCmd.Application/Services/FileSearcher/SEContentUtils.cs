using System.IO;
using Application.Core.Models;

namespace Application;

public class SEContentUtils
{

    public static SearchResult parseResultEntries(SearchResult result, string contentRequest) 
    {
        foreach (var line in File.ReadLines(result.FilePath))
        {
            // TODO: parse user input and find content with Glob or somesing else
        }
        return null;
    }
    
}
