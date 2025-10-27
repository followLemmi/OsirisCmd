using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Application.Core.Models;
using Application.Services.FileSearcher.Settings;
using Application.Services.Utils;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using DotNet.Globbing;
using Serilog;
using Directory = System.IO.Directory;
using Document = Lucene.Net.Documents.Document;

namespace Application.Services.FileSearcher;

public class SearchingEngine
{
    private readonly FileSearcherSettings? _settings;

    private readonly string _indexPath;
    private readonly OsirisCmdLuceneAnalyzer _analyzer;
    private IndexWriter? _indexWriter;
    private DirectoryReader? _directoryReader;
    private IndexSearcher? _indexSearcher;
    
    // Debug purposes
    private readonly List<DirectoryInfo> _skippedDirectories = [];

    public SearchingEngine(FileSearcherSettings fileSearcherSettings)
    {
        _settings = fileSearcherSettings;
        _indexPath = _settings.GetPathToIndexes();
        _analyzer = new OsirisCmdLuceneAnalyzer();
        InitializeIndex();
    }

    private void IndexFile(string filePath, string content)
    {
        var document = new Document();
        var fileInfo = new FileInfo(filePath);

        if (!fileInfo.Exists)
        {
            return;
        }

        document.Add(new StringField("fullPath", filePath, Field.Store.YES));
        document.Add(new TextField("fileName", fileInfo.Name, Field.Store.YES));
        document.Add(new TextField("extension", fileInfo.Extension.ToLower(), Field.Store.YES));

        if (!string.IsNullOrEmpty(content))
        {
            document.Add(new TextField("content", content.ToLower(), Field.Store.NO));
        }

        document.Add(new Int64Field("fileSize", fileInfo.Length, Field.Store.YES));
        document.Add(new Int64Field("lastModified", fileInfo.LastWriteTime.Ticks, Field.Store.YES));

        var term = new Term("fullPath", filePath);
        _indexWriter?.UpdateDocument(term, document);
    }

    public bool IsIndexEmpty()
    {
        var searcher = GetSearcher();
        if (searcher == null)
        {
            return true;
        }

        return searcher.IndexReader.NumDocs == 0;
    }

    private IndexSearcher? GetSearcher()
    {
        if (_indexSearcher == null)
        {
            RefreshSearcher();
        }

        return _indexSearcher;
    }

    private void Commit()
    {
        _indexWriter!.ForceMerge(5);
        _indexWriter.Commit();
        RefreshSearcher();
    }

    private void InitializeIndex()
    {
        EnsureIndexDirectoryIsAccessible(_indexPath);
        var directory = FSDirectory.Open(_indexPath);
        var config = new IndexWriterConfig(LuceneVersion.LUCENE_48, _analyzer)
        {
            UseCompoundFile = true,
            RAMBufferSizeMB = 256
        };
        _indexWriter = new IndexWriter(directory, config);
    }

    private static void EnsureIndexDirectoryIsAccessible(string indexPath)
    {
        var lockFilePath = Path.Combine(indexPath, "write.lock");
        if (File.Exists(lockFilePath))
        {
            try
            {
                File.Delete(lockFilePath);
            }
            catch (Exception ex)
            {
                Log.Warning($"Could not delete lock file: {ex.Message}");
            }
        }
    }

    private void CloseSearcher()
    {
        _directoryReader?.Dispose();
        _indexSearcher = null;
    }

    private void RefreshSearcher()
    {
        _directoryReader?.Dispose();
        try
        {
            _directoryReader = DirectoryReader.Open(_indexWriter.Directory);
            _indexSearcher = new IndexSearcher(_directoryReader);
        }
        catch (IndexNotFoundException e)
        {
            _directoryReader = null;
            _indexSearcher = null;
        }
    }


    public List<SearchResult> ExecuteSearch(BooleanQuery query, string fileNameRequest, string contentRequest, int maxResults, SearchOptions searchOptions)
    {
        var results = new List<SearchResult>();

        var searcher = GetSearcher();
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
                CollapsedFilePath = doc.Get("fullPath"),
                FileName = doc.Get("fileName"),
                Extension = doc.Get("extension"),
                FileSize = long.Parse(doc.Get("fileSize") ?? "0"),
                LastModified = new DateTime(long.Parse(doc.Get("lastModified") ?? "0")),
                Content = doc.Get("content"),
                Score = scoreDoc.Score
            });
        }

        // filter by file name
        if (searchOptions.IsFileNameCaseSensitive)
        {
            results = results.Where(r =>
            {
                var glob = Glob.Parse(fileNameRequest);
                return glob.IsMatch(r.FileName);
            }).ToList();
        }

        // filter by content
        if (!string.IsNullOrEmpty(contentRequest) && !string.IsNullOrWhiteSpace(contentRequest)) 
        {
            foreach (var result in results.ToList())
            {
                var contentEntries = SEContentUtils.ParseResultEntries(result.FilePath, contentRequest, searchOptions.IsContentCaseSensitive);
                if (contentEntries.Count == 0)
                {
                    results.Remove(result);
                }
                else
                {
                    result.ContentEntries = contentEntries;
                }
            }
        }
        CloseSearcher();
        return results.OrderByDescending(x => x.Score).ToList();
    }

    public async void RegularStartIndexing()
    {
        try
        {
            var startTimestamp = DateTime.Now;
            var drivesToIndex = GetDrivesToIndex();
            using var filesCollection = new BlockingCollection<string>();
            using var cts = new CancellationTokenSource();

            var collectingTask = Task.Run(async () =>
            {
                var collectingTasks = drivesToIndex.Select(rootPath =>
                        Task.Run(() => GetAllFilesRecursiveBlocking(filesCollection, rootPath, cts.Token), cts.Token))
                    .ToArray();

                await Task.WhenAll(collectingTasks);
                filesCollection.CompleteAdding(); 
            }, cts.Token);

            var filesCount = 0;
            const int indexingThreads = 12;
            var indexingTasks = Enumerable.Range(0, indexingThreads)
                .Select(_ => Task.Run(() =>
                {
                    foreach (var file in filesCollection.GetConsumingEnumerable(cts.Token))
                    {
                        if (CheckIsFileAlreadyIndexed(file))
                        {
                            continue;
                        }
                        filesCount += 1;
                        try
                        {
                            IndexSingleFile(file);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error indexing file {file}: {ex.Message}");
                        }
                    }
                }, cts.Token)).ToArray();

            await collectingTask;
            await Task.WhenAll(indexingTasks);

            await Task.Run(() =>
            {
                Console.WriteLine("Starting commit");
                Commit();
                var endTimestamp = DateTime.Now;
                _skippedDirectories.ForEach(dir => Console.WriteLine($"Skipped directory: {dir.FullName}"));
                Console.WriteLine($"Skipped directories count: {_skippedDirectories.Count}");
                Console.WriteLine($"Files indexed: {filesCount}");
                Console.WriteLine($"Indexing took {(endTimestamp - startTimestamp).TotalMinutes} minutes");
                Console.WriteLine("Indexing complete!");
            });
        }
        catch (Exception e)
        {
            Log.Error(e, "Error while indexing");
        }
    }

    private bool CheckIsFileAlreadyIndexed(string filePath)
    {
        var fileInfo = new FileInfo(filePath);
        var boolQuery = new BooleanQuery();
        var exactPathQuery = new TermQuery(new Term("fullPath", filePath));
        boolQuery.Add(exactPathQuery, Occur.MUST);
        var searcher = GetSearcher();
        if (searcher == null)
        {
            return false;
        }
        var topDocs = searcher.Search(boolQuery, 1);
        if (topDocs.ScoreDocs.Length != 1)
        {
            return false;
        }
        var doc = searcher.Doc(topDocs.ScoreDocs[0].Doc);
        var lastModified = new DateTime(long.Parse(doc.Get("lastModified") ?? "0"));
        return lastModified.Equals(fileInfo.LastWriteTime);
    }

    public async void CleanIndexes(object? state)
    {
        try
        {
            await Task.Run(() =>
            {
                var searcher = GetSearcher();
                var documentsToDelete = new List<Term>();
                var leaves = searcher.IndexReader.Leaves;
                foreach (var context in leaves)
                {
                    var reader = context.Reader;
                    var liveDocs = context.AtomicReader.LiveDocs;
                    for (int i = 0; i < reader.MaxDoc; i++)
                    {
                        if (liveDocs != null && !liveDocs.Get(i))
                        {
                            continue;
                        }
                        var doc = reader.Document(i);
                        var filePath = doc.Get("fullPath");
                        var fileInfo = new FileInfo(filePath);
                        if (!fileInfo.Exists)
                        {
                            documentsToDelete.Add(new Term("fullPath", filePath));
                        }
                    }
                }

                foreach (var term in documentsToDelete)
                {
                    _indexWriter?.DeleteDocuments(term);
                    Console.WriteLine($"Deleted doc from index {term.Text}");
                }
                Commit();
                Console.WriteLine($"Deleted docs from indexes {documentsToDelete.Count}");
                CloseSearcher();
                return Task.CompletedTask;
            });
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception during CLEAN INDEXES {e}");
        }
    }

    public async void FirstStartIndexing()
    {
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (Directory.Exists(_indexPath))
                {
                    Directory.Delete(_indexPath, true);
                }
            }
            try
            {
                var startTimestamp = DateTime.Now;
                var drivesToIndex = GetDrivesToIndex();
                using var filesCollection = new BlockingCollection<string>();
                using var cts = new CancellationTokenSource();

                var collectingTask = Task.Run(async () =>
                {
                    var collectingTasks = drivesToIndex.Select(rootPath =>
                            Task.Run(() => GetAllFilesRecursiveBlocking(filesCollection, rootPath, cts.Token), cts.Token))
                        .ToArray();

                    await Task.WhenAll(collectingTasks);
                    filesCollection.CompleteAdding(); 
                }, cts.Token);

                var filesCount = 0;
                const int indexingThreads = 12;
                var indexingTasks = Enumerable.Range(0, indexingThreads)
                    .Select(_ => Task.Run(() =>
                    {
                        foreach (var file in filesCollection.GetConsumingEnumerable(cts.Token))
                        {
                            filesCount += 1;
                            try
                            {
                                IndexSingleFile(file);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error indexing file {file}: {ex.Message}");
                            }
                        }
                    }, cts.Token)).ToArray();

                await collectingTask;
                await Task.WhenAll(indexingTasks);

                await Task.Run(() =>
                {
                    Console.WriteLine("Starting commit");
                    Commit();
                    var endTimestamp = DateTime.Now;
                    _skippedDirectories.ForEach(dir => Console.WriteLine($"Skipped directory: {dir.FullName}"));
                    Console.WriteLine($"Skipped directories count: {_skippedDirectories.Count}");
                    Console.WriteLine($"Files indexed: {filesCount}");
                    Console.WriteLine($"Indexing took {(endTimestamp - startTimestamp).TotalMinutes} minutes");
                    Console.WriteLine("Indexing complete!");
                });
            }
            catch (Exception e)
            {
                Log.Error(e, "Error while indexing");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception during FIRST START INDEXING {e}");
        }
    }
    
    private void GetAllFilesRecursiveBlocking(BlockingCollection<string> filesCollection, string directoryPath,
        CancellationToken cancellationToken)
    {
        var queue = new Queue<string>();
        queue.Enqueue(directoryPath);

        while (queue.Count > 0 && !cancellationToken.IsCancellationRequested)
        {
            var currentDirectory = queue.Dequeue();

            var dirInfo = new DirectoryInfo(currentDirectory);
            if (dirInfo.Attributes.HasFlag(FileAttributes.ReparsePoint) ||
                ShouldSkipDirectory(currentDirectory) ||
                !HasDirectoryAccess(currentDirectory))
            {
                continue;
            }

            try
            {
                foreach (var file in Directory.EnumerateFiles(currentDirectory))
                {
                    filesCollection.Add(file, cancellationToken);
                }

                foreach (var subdirectory in Directory.EnumerateDirectories(currentDirectory))
                {
                    queue.Enqueue(subdirectory);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing directory {currentDirectory}: {ex.Message}");
            }
        }
    }

    private List<string> GetDrivesToIndex()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return new List<string>()
            {
                "/"
            };
        }

        var rootDirectoriesToIndex = new List<string>();
        var drives = DriveInfo.GetDrives();
        foreach (var drive in drives)
        {
            var needToIndex = true;
            foreach (var settingDrive in _settings?.GetDrivesToIndex()!)
            {
                if (settingDrive.Name.Equals(drive.Name, StringComparison.InvariantCultureIgnoreCase) &&
                    !settingDrive.Enabled)
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

    private void IndexSingleFile(string file)
    {
        var fileInfo = new FileInfo(file);
        if (!fileInfo.Exists)
        {
            return;
        }

        Console.WriteLine($"Indexing {file}");

        var content = GetFileContent(file);

        IndexFile(file, content);
    }

    private bool HasDirectoryAccess(string directoryPath)
    {
        try
        {
            // Here we check directory access. If EnumerateFileSystemEntries throw an exception - we not have access to this directory
            Directory.EnumerateFileSystemEntries(directoryPath).Take(1).ToList();
            return true;
        }
        catch (UnauthorizedAccessException)
        {
            _skippedDirectories.Add(new DirectoryInfo(directoryPath));
            return false;
        }
        catch (DirectoryNotFoundException)
        {
            _skippedDirectories.Add(new DirectoryInfo(directoryPath));
            return false;
        }
        catch (Exception)
        {
            _skippedDirectories.Add(new DirectoryInfo(directoryPath));
            return false;
        }
    }

    private string GetFileContent(string filePath)
    {
        var fileName = Path.GetFileName(filePath);
        var extension = Path.GetExtension(filePath);

        if (!_settings!.GetReadContentExtensions().Contains(extension) &&
            !_settings.GetReadContentFiles().Contains(fileName))
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
