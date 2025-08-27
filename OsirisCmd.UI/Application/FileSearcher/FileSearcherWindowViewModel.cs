using System.Collections.ObjectModel;
using Microsoft.Extensions.DependencyInjection;
using OsirisCmd.Core.Models;
using OsirisCmd.Core.Services.FileSearcher;
using OsirisCmd.Services.Services.FileSearcher;

namespace OsirisCmd.UI.Application.FileSearcher;

public class FileSearcherWindowViewModel : ViewModelBase
{
    private IFileSearcherService _fileSearcherService;
    
    public ObservableCollection<SearchResult> SearchResults { get; set; } = [];
    
    public FileSearcherWindowViewModel()
    {
        _fileSearcherService = UIServiceProviderAdapter.ServiceProvider.GetRequiredService<IFileSearcherService>();
    }

    public async void OnSearchTextChanged(string searchText)
    {
        SearchResults.Clear();
        var results = new ObservableCollection<SearchResult>(_fileSearcherService.SearchByFileContent(searchText));
        foreach (var result in results)
        {
            SearchResults.Add(result);
        }
        Console.WriteLine(SearchResults.Count);
    }
}