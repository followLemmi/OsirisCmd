using OsirisCmd.Core.Models;

namespace OsirisCmd.Core.Services.FileSearcher;

public interface IFileSearcherService
{

    List<SearchResult> SearchByFileName(string fileName, int maxResults = 100);

    List<SearchResult> SearchByFileContent(string content, int maxResults = 100);

}