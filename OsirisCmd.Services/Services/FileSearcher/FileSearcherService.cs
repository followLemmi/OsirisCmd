using System.Collections.Concurrent;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Util;
using OsirisCmd.Core.Models;
using OsirisCmd.Core.Services.FileSearcher;
using OsirisCmd.Core.Services.Logger;
using OsirisCmd.Core.Services.SettingsManager;
using OsirisCmd.Services.Services.FileSearcher.Settings;
using Serilog;

namespace OsirisCmd.Services.Services.FileSearcher;

public class FileSearcherService : IFileSearcherService
{
    private ILoggerService _logger;

    private ConcurrentQueue<string> _filesToIndex = new();
    
    private readonly SearchingEngine.SearchingEngine _searchingEngine;
    private readonly QueryParser _fileNameParser;
    private readonly QueryParser _fileContentParser;
    private readonly MultiFieldQueryParser _multiFieldParser;

    private readonly FileSearcherSettings? _settings;
    
    public FileSearcherService(ILoggerService logger, ISettingsProviderService? settingsProvider)
    {
        _logger = logger;
        _searchingEngine = new SearchingEngine.SearchingEngine(settingsProvider);
        var analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);
        _fileNameParser = new QueryParser(LuceneVersion.LUCENE_48, "fileName", analyzer);
        _fileContentParser = new QueryParser(LuceneVersion.LUCENE_48, "content", analyzer);
        _multiFieldParser = new MultiFieldQueryParser(LuceneVersion.LUCENE_48, ["fileName", "content"], analyzer);

        _searchingEngine.StartupIndexing();
    }

    public List<SearchResult> SearchByFileName(string fileName, int maxResults = 100)
    {
        try
        {
            _logger.LogDebug($"Search by file name: {fileName}");
            var query = _fileNameParser.Parse(fileName);
            return _searchingEngine.ExecuteSearch(query, maxResults);
        } catch (Exception e)
        {
            Console.WriteLine(e);
            return [];
        }
    }

    public List<SearchResult> SearchByFileContent(string content, int maxResults = 100)
    {
        try
        {
            _logger.LogDebug($"Search by file content: {content}");
            var query = _fileContentParser.Parse(content);
            return _searchingEngine.ExecuteSearch(query, maxResults);
        } catch (Exception e)
        {
            Console.WriteLine(e);
            return [];
        }
    }


}
