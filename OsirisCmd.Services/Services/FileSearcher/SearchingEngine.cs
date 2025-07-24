using System.Reflection.Metadata;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using OsirisCmd.Core.Models;
using OsirisCmd.Core.Services.SettingsManager;
using OsirisCmd.Services.Services.FileSearcher.Settings;
using Serilog;
using Directory = System.IO.Directory;
using Document = Lucene.Net.Documents.Document;

namespace OsirisCmd.SearchingEngine;

public class SearchingEngine
{
    private readonly FileSearcherSettings? _settings;

    private readonly string _indexPath;
    private readonly StandardAnalyzer _analyzer;
    private IndexWriter _indexWriter;
    private DirectoryReader? _directoryReader;
    private IndexSearcher? _indexSearcher;
    private List<FileInfo> _files = [];

    public SearchingEngine(ISettingsProviderService settingsProvider)
    {
        _settings = settingsProvider.AttachSettings<FileSearcherSettings>();
        _indexPath = _settings.GetPathToIndexes();
        _analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);
        InitializeIndex();
    }
    
    public void IndexFile(string filePath, string content)
    {
        var document = new Document();
        var fileInfo = new FileInfo(filePath);

        if (!fileInfo.Exists)
        {
            return;
        }
        
        document.Add(new StringField("fullPath", filePath, Field.Store.YES));
        document.Add(new TextField("fileName", fileInfo.Name, Field.Store.YES));
        document.Add(new TextField("fileNameNoExt", Path.GetFileNameWithoutExtension(filePath), Field.Store.NO));
        document.Add(new TextField("extension", fileInfo.Extension.ToLower(), Field.Store.YES));
        document.Add(new TextField("directory", fileInfo.DirectoryName, Field.Store.NO));
        
        if (!string.IsNullOrEmpty(content))
        {
            document.Add(new TextField("content", content, Field.Store.NO));
        }
        
        document.Add(new Int64Field("fileSize", fileInfo.Length, Field.Store.YES));
        document.Add(new Int64Field("lastModified", fileInfo.LastWriteTime.Ticks, Field.Store.YES));
        
        var term = new Term("fullPath", filePath);
        _indexWriter.UpdateDocument(term, document);
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

    public IndexSearcher? GetSearcher()
    {
        if (_indexSearcher == null)
        {
            RefreshSearcher();
        }
        return _indexSearcher;
    }

    public void Commit()
    {
        _indexWriter.ForceMerge(1);
        _indexWriter.Commit();
        RefreshSearcher();
    }

    public void Dispose()
    {
        _indexWriter.Dispose();
        _directoryReader?.Dispose();
        _analyzer.Dispose();
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

    
    private void EnsureIndexDirectoryIsAccessible(string indexPath)
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
    
    
    public List<SearchResult> ExecuteSearch(Query query, int maxResults)
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
                FileName = doc.Get("fileName"),
                Extension = doc.Get("extension"),
                FileSize = long.Parse(doc.Get("fileSize") ?? "0"),
                LastModified = new DateTime(long.Parse(doc.Get("lastModified") ?? "0")),
                Score = scoreDoc.Score
            });
        }
        return results.OrderByDescending(x => x.Score).ToList();
    }
    
    
    public async void StartupIndexing()
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
                Commit();
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

        IndexFile(file, content);

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
