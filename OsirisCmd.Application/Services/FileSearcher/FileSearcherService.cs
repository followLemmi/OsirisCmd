using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Application.Core.Models;
using Application.Core.Services.FileSearcher;
using Application.Core.Services.Logger;
using Application.Core.Services.SettingsManager;
using Application.Services.FileSearcher.Settings;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Util;

namespace Application.Services.FileSearcher;

public class FileSearcherService : IFileSearcherService
{
    private readonly ILoggerService _logger;

    private readonly SearchingEngine _searchingEngine;

    private readonly FileSearcherSettings? _settings;

    public FileSearcherService(ILoggerService logger, ISettingsProviderService settingsProvider)
    {
        ArgumentNullException.ThrowIfNull(settingsProvider);
        _logger = logger;
        _settings = settingsProvider!.AttachSettings<FileSearcherSettings>();
        _searchingEngine = new SearchingEngine(_settings!);

        // if (_settings != null && _settings.IsFileIndexingEnabled())
        // {
        // }
        // _searchingEngine.StartupIndexing();
    }

    public List<SearchResult> SmartSearch(string fileName, string content, SearchOptions searchOptions, int maxResults = 100)
    {
        try
        {
            _logger.LogDebug($"Search by file name = {fileName} and file content = {content}");
            
            var boolQuery = new BooleanQuery();

            if (!string.IsNullOrWhiteSpace(fileName))
            {
                var fileNameQuery = CreateQuery(fileName, "fileName", searchOptions);
                boolQuery.Add(fileNameQuery, Occur.MUST);
            }

            if (!string.IsNullOrWhiteSpace(content))
            {
                var contentQuery = CreateQuery(content, "content", null); //TODO: add support of search options
                boolQuery.Add(contentQuery, Occur.MUST);
            }
            return _searchingEngine.ExecuteSearch(boolQuery, fileName, content, maxResults, searchOptions);
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
        if (searchOptions.IsFileNameCaseSensitive || searchOptions.IsContentCaseSensitive)
        {
            return new TermQuery(new Term(field, userInput.ToLower()));
        }
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
