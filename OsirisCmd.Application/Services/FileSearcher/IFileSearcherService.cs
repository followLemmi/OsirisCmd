using System.Collections.Generic;
using Application.Core.Models;
using Application.Services.FileSearcher;

namespace Application.Core.Services.FileSearcher;

public interface IFileSearcherService
{

    List<SearchResult> SmartSearch(string fileName, string content, SearchOptions searchOptions, int maxResults = 100);

}
