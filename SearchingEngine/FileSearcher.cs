using Lucene.Net.Analysis.Standard;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Util;

namespace SearchingEngine;

public class FileSearcher
{
    private readonly SearchingEngine _searchingEngine;
    private readonly QueryParser _fileNameParser;
    private readonly QueryParser _fileContentParser;
    private readonly MultiFieldQueryParser _multiFieldParser;
    
    private static readonly HashSet<string> TextExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".txt", ".cs", ".js", ".html", ".htm", ".xml", ".json", ".css", ".sql", 
        ".py", ".java", ".cpp", ".c", ".h", ".hpp", ".md", ".yml", ".yaml", 
        ".ini", ".cfg", ".conf", ".log", ".csv", ".tsv", ".bat", ".sh", ".ps1",
        ".xaml", ".csproj", ".sln", ".config", ".properties", ".dockerfile",
        ".gitignore", ".gitattributes", ".editorconfig", ".proto", ".razor",
        ".vue", ".ts", ".tsx", ".jsx", ".scss", ".less", ".sass", ".php",
        ".rb", ".go", ".rs", ".kt", ".scala", ".swift", ".r", ".m", ".pl"
    };
    
    private static readonly HashSet<string> BinaryExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".exe", ".dll", ".so", ".dylib", ".bin", ".dat", ".db", ".sqlite",
        ".zip", ".rar", ".7z", ".tar", ".gz", ".bz2", ".xz", ".deb", ".rpm",
        ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".ico", ".webp", ".svg", ".tiff",
        ".mp3", ".mp4", ".avi", ".mkv", ".mov", ".wmv", ".flv", ".wav", ".flac",
        ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".odt", ".ods",
        ".class", ".jar", ".war", ".ear", ".pyc", ".o", ".obj", ".lib", ".a",
        ".iso", ".img", ".dmg", ".msi", ".pkg", ".snap", ".flatpak", ".appimage"
    };
    
    private static readonly HashSet<string> SkipDirectories = new(StringComparer.OrdinalIgnoreCase)
    {
        // Unix/Linux системные папки
        "proc", "sys", "dev", "run", "tmp", "/var/tmp", "/var/cache",
        "/var/spool", "/var/lock", "/var/run", "boot", "lost+found", "snap",
    
        // Папки пользователя Unix
        ".cache", ".thumbnails", ".local/share/Trash",
    
        // Windows системные папки  
        "System32", "SysWOW64", "WinSxS", "Temp", "Logs", 
        "System Volume Information", "$Recycle.Bin", "Windows",
    
        // Папки разработки
        "node_modules", ".git", ".svn", ".hg", "bin", "obj", 
        "build", "dist", "out", ".vs", ".idea", "packages", 
        "vendor", "__pycache__", ".pytest_cache", ".tox", "coverage", 
        ".nyc_output", ".gradle", ".ivy2", ".sbt",
    
        // Дополнительные кэши и временные папки
        "cache", "Cache", "tmp", "temp", "Temp", "logs", "Logs"
    };

    private static readonly HashSet<string> SkipPatterns = new(StringComparer.OrdinalIgnoreCase)
    {
        // Кэши браузеров
        "mozilla", "chrome", "chromium", "firefox", "safari", "edge",
    
        // IDE и редакторы
        "jetbrains", "intellij", "pycharm", "webstorm", "rider",
    
        // Системные папки
        "system volume information", "windows.old", "programdata",
    
        // Snap пакеты
        "snap"
    };

    public FileSearcher(string indexStoragePath)
    {
        _searchingEngine = new SearchingEngine(indexStoragePath);
        // IndexFiles();
        var analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);
        _fileNameParser = new QueryParser(LuceneVersion.LUCENE_48, "fileName", analyzer);
        _fileContentParser = new QueryParser(LuceneVersion.LUCENE_48, "content", analyzer);
        _multiFieldParser = new MultiFieldQueryParser(LuceneVersion.LUCENE_48, 
            new[] {"fileName", "content"}, analyzer);
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
    
    private void IndexFiles()
    {
        var rootPath = "c://";
        Console.WriteLine($"Strat indexing in {rootPath}...");
        
        try
        {
            IndexDirectory(rootPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while indexing: {ex.Message}");
        }
        
        _searchingEngine.Commit();
        Console.WriteLine("Indexing complete!");
    }

    private void IndexDirectory(string directoryPath)
    {
        try
        {
            if (ShouldSkipDirectory(directoryPath))
            {
                Console.WriteLine($"Skip directory: {directoryPath}");
                return;
            }
            
            if (!HasDirectoryAccess(directoryPath))
            {
                Console.WriteLine($"No permission to directory: {directoryPath}");
                return;
            }
            
            try
            {
                var files = Directory.EnumerateFiles(directoryPath);
                foreach (var file in files)
                {
                    try
                    {
                        IndexSingleFile(file);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        Console.WriteLine($"No permission to file: {file}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error indexing {file}: {ex.Message}");
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"No permission to directory: {directoryPath}");
                return;
            }
            
            try
            {
                var subdirectories = Directory.EnumerateDirectories(directoryPath);
                foreach (var subdirectory in subdirectories)
                {
                    IndexDirectory(subdirectory);
                }
            }
            catch (UnauthorizedAccessException)
            {
                Console.WriteLine($"No permission to directory: {directoryPath}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while processing directory {directoryPath}: {ex.Message}");
        }
    }

    private void IndexSingleFile(string file)
    {
        var fileInfo = new FileInfo(file);

        if (_searchingEngine.IsFileIndexed(file, fileInfo.LastWriteTime))
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
        var extension = Path.GetExtension(filePath);
        
        // if (BinaryExtensions.Contains(extension))
        // {
        //     return "";
        // }

        if (TextExtensions.Contains(extension))
        {
            return ReadTextFileContent(filePath);
        }

        if (string.IsNullOrEmpty(extension) || IsTextFile(filePath))
        {
            return ReadTextFileContent(filePath);
        }

        return "";
    }
    
    private string ReadTextFileContent(string filePath)
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
    
    private bool IsTextFile(string filePath, int sampleSize = 512)
    {
        try
        {
            using var fileStream = File.OpenRead(filePath);
            var buffer = new byte[Math.Min(sampleSize, (int)fileStream.Length)];
            int bytesRead = fileStream.Read(buffer, 0, buffer.Length);
            
            if (bytesRead == 0) return true;
            
            for (int i = 0; i < bytesRead; i++)
            {
                if (buffer[i] == 0) return false;
            }
            
            int printableCount = 0;
            for (int i = 0; i < bytesRead; i++)
            {
                byte b = buffer[i];
                if (IsPrintableOrWhitespace(b))
                {
                    printableCount++;
                }
            }
            
            double printableRatio = (double)printableCount / bytesRead;
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
        var dirName = Path.GetFileName(directoryPath);
        var fullPath = Path.GetFullPath(directoryPath);
        
        var unixSystemDirs = new[] { "/proc", "/sys", "/dev", "/run", "/tmp", "/var", "/snap", "/usr" };
        if (unixSystemDirs.Any(sysDir => fullPath.StartsWith(sysDir + "/") || fullPath == sysDir))
        {
            return true;
        }
        
        if (SkipDirectories.Contains(dirName))
        {
            return true;
        }
        
        if (dirName.StartsWith(".") && !IsImportantHiddenDirectory(dirName))
        {
            return true;
        }
        
        if (dirName.Contains("cache", StringComparison.OrdinalIgnoreCase) ||
            dirName.Contains("temp", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }
    
        return false;
    }

    private static bool IsImportantHiddenDirectory(string dirName)
    {
        // Некоторые скрытые папки могут содержать полезные файлы
        var importantHiddenDirs = new[] { 
            ".config", ".local", ".ssh", ".bashrc", ".vimrc", 
            ".profile", ".gitconfig", ".npmrc", ".dockerenv" 
        };
        return importantHiddenDirs.Contains(dirName, StringComparer.OrdinalIgnoreCase);
    }


}
