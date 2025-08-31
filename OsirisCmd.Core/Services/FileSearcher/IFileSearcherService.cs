using OsirisCmd.Core.Models;

namespace OsirisCmd.Core.Services.FileSearcher;

public interface IFileSearcherService
{

    List<SearchResult> SmartSearch(string fileName, string content, int maxResults = 100);

}
