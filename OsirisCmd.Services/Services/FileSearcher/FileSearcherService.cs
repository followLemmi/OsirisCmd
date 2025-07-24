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
        _settings = settingsProvider!.AttachSettings<FileSearcherSettings>();
        _searchingEngine = new SearchingEngine.SearchingEngine(_settings!.GetPathToIndexes());
        var analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);
        _fileNameParser = new QueryParser(LuceneVersion.LUCENE_48, "fileName", analyzer);
        _fileContentParser = new QueryParser(LuceneVersion.LUCENE_48, "content", analyzer);
        _multiFieldParser = new MultiFieldQueryParser(LuceneVersion.LUCENE_48, ["fileName", "content"], analyzer);

        StartupIndexing();
    }

    public List<SearchResult> SearchByFileName(string fileName, int maxResults = 100)
    {
        try
        {
            var query = _fileNameParser.Parse(fileName);
            return ExecuteSearch(query, maxResults);
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
            var query = _fileContentParser.Parse(content);
            return ExecuteSearch(query, maxResults);
        } catch (Exception e)
        {
            Console.WriteLine(e);
            return [];
        }
    }

    private List<SearchResult> ExecuteSearch(Query query, int maxResults)
    {
        var results = new List<SearchResult>();
        
        var searcher = _searchingEngine.GetSearcher();
        if (searcher == null)
        {
            return results;
        }
        
        var topDocs = searcher.Search(query, maxResults);
        foreach (var scoreDoc in topDocs.ScoreDocs)
        {
            var doc = searcher.Doc(scoreDoc.Doc);
            results.Add(new SearchResult
            {
                FilePath = doc.Get("fullPath"),
                FileName = doc.Get("fileName"),
                Extension = doc.Get("extension"),
                FileSize = long.Parse(doc.Get("fileSize") ?? "0"),
                LastModified = new DateTime(long.Parse(doc.Get("lastModified") ?? "0")),
                Score = scoreDoc.Score
            });
        }
        return results.OrderByDescending(x => x.Score).ToList();
    }
    
    
    private async void StartupIndexing()
    {
        try
        {
            var startTimestamp = DateTime.Now;
            var tasks = new List<Task>();
            var drivesToIndex = GetDrivesToIndex();
            var allFiles = new List<string>();
            try
            {
                tasks.AddRange(drivesToIndex.Select(rootPath => Task.Run(() => { allFiles.AddRange(GetAllFilesRecursive(rootPath)); })));

                await Task.WhenAll(tasks);
            
                const int batchSize = 1000;
                var batches = allFiles
                    .Select((file, index) => new { file, index })
                    .GroupBy(x => x.index / batchSize)
                    .Select(g => g.Select(x => x.file).ToList())
                    .ToList();
            
                tasks = batches.Select(batch => Task.Run(() => IndexFilesBatch(batch))).ToList();
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while indexing: {ex.Message}");
            }

            await Task.Run(() =>
            {
                _searchingEngine.Commit();
                var endTimestamp = DateTime.Now;
                Console.WriteLine($"Indexing took {(endTimestamp - startTimestamp).TotalMinutes} minutes");
                Console.WriteLine("Indexing complete!");
            });
        }
        catch (Exception e)
        {
            Log.Error(e, "Error while indexing");
        }
    }

    private List<string> GetDrivesToIndex()
    {
        var rootDirectoriesToIndex = new List<string>();
        var drives = DriveInfo.GetDrives();
        foreach (var drive in drives)
        {
            var needToIndex = true;
            foreach (var settingDrive in _settings?.GetDrivesToIndex()!)
            {
                if (settingDrive.Name.Equals(drive.Name, StringComparison.InvariantCultureIgnoreCase) && !settingDrive.Enabled)
                {
                    needToIndex = false;
                }
            }

            if (needToIndex)
            {
                rootDirectoriesToIndex.Add(drive.RootDirectory.FullName);
            }
        }
        return rootDirectoriesToIndex;
    }

    private List<string> GetAllFilesRecursive(string directoryPath)
    {
        var allFiles = new List<string>();
        var queue = new Queue<string>();
        queue.Enqueue(directoryPath);
        while (queue.Count > 0)
        {
            var currentDirectory = queue.Dequeue();
            if (ShouldSkipDirectory(currentDirectory) || !HasDirectoryAccess(currentDirectory))
            {
                continue;
            }
            
            var files = Directory.EnumerateFiles(currentDirectory);
            allFiles.AddRange(files);
            
            var subdirectories = Directory.EnumerateDirectories(currentDirectory);
            foreach (var subdirectory in subdirectories)
            {
                queue.Enqueue(subdirectory);
            }
        }
        return allFiles;
    }

    private void IndexFilesBatch(List<string> files)
    {
        var parallelOption = new ParallelOptions()
        {
            MaxDegreeOfParallelism = 128
        };
        Parallel.ForEach(files, parallelOption, IndexSingleFile);
    }

    private void IndexSingleFile(string file)
    {
        var fileInfo = new FileInfo(file);
        if (!fileInfo.Exists)
        {
            return;
        }
        
        Console.WriteLine($"Indexing {file}");
        
        var content = GetFileContent(file);

        _searchingEngine.IndexFile(file, content);

    }

    private static bool HasDirectoryAccess(string directoryPath)
    {
        try
        {
            var canTakeDir = Directory.EnumerateFileSystemEntries(directoryPath).Take(1).ToList();
            return true;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
        catch (DirectoryNotFoundException)
        {
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private string GetFileContent(string filePath)
    {
        var fileName = Path.GetFileName(filePath);
        var extension = Path.GetExtension(filePath);
        
        if (!_settings!.GetReadContentExtensions().Contains(extension) || !_settings.GetReadContentFiles().Contains(fileName))
        {
            return "";
        }

        if (string.IsNullOrEmpty(extension) || IsTextFile(filePath))
        {
            return ReadTextFileContent(filePath);
        }

        return "";
    }
    
    private static string ReadTextFileContent(string filePath)
    {
        try
        {
            return File.ReadAllText(filePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error file reading {filePath}: {ex.Message}");
            return "";
        }
    }
    
    private static bool IsTextFile(string filePath, int sampleSize = 512)
    {
        try
        {
            using var fileStream = File.OpenRead(filePath);
            var buffer = new byte[Math.Min(sampleSize, (int)fileStream.Length)];
            var bytesRead = fileStream.Read(buffer, 0, buffer.Length);
            
            if (bytesRead == 0) return true;
            
            for (var i = 0; i < bytesRead; i++)
            {
                if (buffer[i] == 0) return false;
            }
            
            var printableCount = 0;
            for (var i = 0; i < bytesRead; i++)
            {
                byte b = buffer[i];
                if (IsPrintableOrWhitespace(b))
                {
                    printableCount++;
                }
            }
            
            var printableRatio = (double)printableCount / bytesRead;
            return printableRatio >= 0.95;
        }
        catch
        {
            return false;
        }
    }

    private static bool IsPrintableOrWhitespace(byte b)
    {
        return (b >= 32 && b <= 126) || //
               b == 9 || 
               b == 10 ||
               b == 13 ||
               b >= 128;
    }

    private bool ShouldSkipDirectory(string directoryPath)
    {
        var fullPath = Path.GetFullPath(directoryPath);
        return _settings!.GetAllDirectoriesToSkip().Any(value => fullPath.Contains(value));
    }

}
