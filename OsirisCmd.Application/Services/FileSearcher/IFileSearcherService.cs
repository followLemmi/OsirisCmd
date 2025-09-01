using System.Collections.Generic;
using Application.Core.Models;

namespace Application.Core.Services.FileSearcher;

public interface IFileSearcherService
{

    List<SearchResult> SmartSearch(string fileName, string content, int maxResults = 100);

}
