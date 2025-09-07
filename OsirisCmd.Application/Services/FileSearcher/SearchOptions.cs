namespace Application.Services.FileSearcher;

public class SearchOptions
{
    public bool IsFileNameCaseSensitive { get; set; }
    public bool IsContentCaseSensitive { get; set; }

    public bool IsAnyFilterAvailable() {
        return IsFileNameCaseSensitive || IsContentCaseSensitive;
    }
}
