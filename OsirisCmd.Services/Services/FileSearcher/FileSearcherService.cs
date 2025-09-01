using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Util;
using OsirisCmd.Core.Models;
using OsirisCmd.Core.Services.FileSearcher;
using OsirisCmd.Core.Services.Logger;
using OsirisCmd.Core.Services.SettingsManager;
using OsirisCmd.Services.Services.FileSearcher.Settings;

namespace OsirisCmd.Services.Services.FileSearcher;

public class FileSearcherService : IFileSearcherService
{
    private readonly ILoggerService _logger;

    private readonly SearchingEngine _searchingEngine;
    private readonly QueryParser _fileNameParser;
    private readonly QueryParser _fileContentParser;
    private readonly MultiFieldQueryParser _multiFieldParser;

    private readonly FileSearcherSettings? _settings;

    public FileSearcherService(ILoggerService logger, ISettingsProviderService settingsProvider)
    {
        ArgumentNullException.ThrowIfNull(settingsProvider);
        _logger = logger;
        _settings = settingsProvider!.AttachSettings<FileSearcherSettings>();
        _searchingEngine = new SearchingEngine(_settings!);
        var analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);
        _fileNameParser = new QueryParser(LuceneVersion.LUCENE_48, "fileName", analyzer);
        _fileContentParser = new QueryParser(LuceneVersion.LUCENE_48, "content", analyzer);
        _multiFieldParser = new MultiFieldQueryParser(LuceneVersion.LUCENE_48, ["fileName", "content"], analyzer);

        // if (_settings != null && _settings.IsFileIndexingEnabled())
        // {
        // }
        // _searchingEngine.StartupIndexing();
    }

    public List<SearchResult> SmartSearch(string fileName, string content, int maxResults = 100)
    {
        try
        {
            _logger.LogDebug($"Search by file name = {fileName} and file content = {content}");
            
            var boolQuery = new BooleanQuery();

            if (!string.IsNullOrWhiteSpace(fileName))
            {
                var fileNameQuery = CreateQuery(fileName, "fileName", null); //TODO: add support of search options
                boolQuery.Add(fileNameQuery, Occur.MUST);
            }

            if (!string.IsNullOrWhiteSpace(content))
            {
                var contentQuery = CreateQuery(content, "content", null); //TODO: add support of search options
                boolQuery.Add(contentQuery, Occur.MUST);
            }
            return _searchingEngine.ExecuteSearch(boolQuery, maxResults);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return [];
        }
    }

    private Query CreateQuery(string query, string field, SearchOptions searchOptions)
    {
        var userInput = query.Trim();
        if (userInput.StartsWith($"\"") && userInput.EndsWith($"\"") && userInput.Length > 2)
        {
            userInput = userInput.Substring(1, userInput.Length - 2);
            return new TermQuery(new Term(field, userInput));
        }

        if (userInput.EndsWith('~'))
        {
            var fuzzyInput = userInput.Substring(0, userInput.Length - 1);
            var parts = fuzzyInput.Split('~');
            if (parts.Length == 2 && int.TryParse(parts[1], out var editDistance))
            {
                return new FuzzyQuery(new Term(field, parts[0]), editDistance);
            }
            return new FuzzyQuery(new Term(field, fuzzyInput), 1);
        }

        if (userInput.Contains('*') || userInput.Contains('?'))
        {
            return new WildcardQuery(new Term(field, userInput.ToLower()));
        }

        if (userInput.Contains(' '))
        {
            var words = userInput.Split(' ');
            var wordsQuery = new BooleanQuery();
            foreach (var word in words)
            {
                wordsQuery.Add(new TermQuery(new Term(field, word.ToLower())), Occur.SHOULD);
            }
            return wordsQuery;
        }
        
        return new WildcardQuery(new Term(field, $"*{userInput.ToLower()}*"));
    }


}
