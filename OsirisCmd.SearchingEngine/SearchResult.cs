namespace OsirisCmd.SearchingEngine;

public class SearchResult
{
    public string FilePath { get; set; }
    public string FileName { get; set; }
    public string Extension { get; set; }
    public long FileSize { get; set; }
    public DateTime LastModified { get; set; }
    public float Score { get; set; }

    public override string ToString()
    {
        return $"{FilePath} ({FileSize:N0} bytes, {LastModified:yyyy-MM-dd HH:mm:ss})";
    }
}