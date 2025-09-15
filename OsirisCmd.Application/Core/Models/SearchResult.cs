using System;
using System.Collections.Generic;

namespace Application.Core.Models;

public class SearchResult
{
    public required string FilePath { get; set; }
    public required string CollapsedFilePath { get; set; }
    public required string FileName { get; set; }
    public required string Extension { get; set; }
    public long FileSize { get; set; }
    public DateTime LastModified { get; set; }
    public float Score { get; set; }
    public string? Content { get; set; }
    public Dictionary<int, Dictionary<string, List<int>>> ContentEntries { get; set; } = new();

    public override string ToString()
    {
        return $"{FilePath} ({FileSize:N0} bytes, {LastModified:yyyy-MM-dd HH:mm:ss})";
    }
}
