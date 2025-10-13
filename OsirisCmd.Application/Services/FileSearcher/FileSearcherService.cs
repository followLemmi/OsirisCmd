using System;
using System.Collections.Generic;
using Application.Core.Models;
using Application.Core.Services.FileSearcher;
using Application.Core.Services.Logger;
using Application.Core.Services.SettingsManager;
using Application.Services.FileSearcher.Settings;
using Application.Services.Utils;
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
        // _searchingEngine.FirstStartIndexing();
        _searchingEngine.RegularStartIndexing();
    }

    public List<SearchResult> SmartSearch(string fileName, string content, SearchOptions searchOptions, int maxResults = 100)
    {
        try
        {
            _logger.LogDebug($"Search by file name = {fileName} and file content = {content}");
            
            var boolQuery = new BooleanQuery();

            if (!string.IsNullOrWhiteSpace(fileName))
            {
                var fileNameQuery = FileSearcherUtils.CreateQuery(fileName, "fileName");
                boolQuery.Add(fileNameQuery, Occur.MUST);
            }

            if (!string.IsNullOrWhiteSpace(content))
            {
                var contentQuery = FileSearcherUtils.CreateQuery(content, "content");
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

    



}
