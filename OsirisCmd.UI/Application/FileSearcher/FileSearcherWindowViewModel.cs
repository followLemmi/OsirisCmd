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

    public async void OnSearchButtonClicked(string fileName)
    {
        SearchResults.Clear();
        var results = new ObservableCollection<SearchResult>(_fileSearcherService.SearchByFileName(fileName));
        foreach (var result in results)
        {
            SearchResults.Add(result);
        }
    }
}