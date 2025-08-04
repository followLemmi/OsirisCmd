namespace OsirisCmd.UI.Application.FileSearcher;

public class SearchTypeViewModel
{
    public SearchType SearchType { get; set; }
    public string Name { get; set; }
    
    public SearchTypeViewModel(SearchType searchType, string name)
    {
        SearchType = searchType;
        Name = name;    
    }

    public override string ToString() => Name;
}