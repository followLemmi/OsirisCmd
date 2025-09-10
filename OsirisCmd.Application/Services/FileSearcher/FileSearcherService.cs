using System;
using System.Collections.Generic;
using Application.Core.Models;
using Application.Core.Services.FileSearcher;
using Application.Core.Services.Logger;
using Application.Core.Services.SettingsManager;
using Application.Services.FileSearcher.Settings;
using Lucene.Net.Index;
using Lucene.Net.Search;

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
                var contentQuery = CreateQuery(content, "content", searchOptions); //TODO: add support of search options
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
        var userInputWords = userInput.Split([' '], StringSplitOptions.RemoveEmptyEntries);

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

        if (!(userInputWords.Length > 1) && (userInput.Contains('*') || userInput.Contains('?')))
        {
            return new WildcardQuery(new Term(field, userInput.ToLower()));
        }

        if (userInputWords.Length > 1)
        {
            var phraseQuery = new PhraseQuery();
            foreach (var word in userInputWords)
            {
                var wordClone = word;
                if (wordClone.Contains('*') || wordClone.Contains('?') || wordClone.Contains('"'))
                {
                    wordClone = wordClone.Trim('*');
                    wordClone = wordClone.Trim('?');
                    wordClone = wordClone.Trim('"');
                }
                phraseQuery.Add(new Term(field, wordClone.ToLower()));
                phraseQuery.Slop = 0;
            }
            return phraseQuery;
        }
        
        return new WildcardQuery(new Term(field, $"*{userInput.ToLower()}*"));
    }



}
