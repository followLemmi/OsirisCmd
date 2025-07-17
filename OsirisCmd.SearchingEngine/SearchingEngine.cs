using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Serilog;

namespace OsirisCmd.SearchingEngine;

public class SearchingEngine
{

    private readonly string _indexPath;
    private readonly StandardAnalyzer _analyzer;
    private IndexWriter _indexWriter;
    private DirectoryReader? _directoryReader;
    private IndexSearcher? _indexSearcher;
    private List<FileInfo> _files = [];

    public SearchingEngine(string indexPath)
    {
        _indexPath = indexPath;
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

}
