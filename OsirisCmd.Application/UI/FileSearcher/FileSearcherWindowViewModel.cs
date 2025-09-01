using System.Collections.ObjectModel;
using Application.Core.Models;
using Application.Core.Services.FileSearcher;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application.UI.FileSearcher;

public class FileSearcherWindowViewModel : ViewModelBase
{
    private IFileSearcherService _fileSearcherService;
    
    public ObservableCollection<SearchResult> SearchResults { get; set; } = [];
    
    public FileSearcherWindowViewModel()
    {
        _fileSearcherService = MainServiceProvider.ServiceProvider.GetRequiredService<IFileSearcherService>();
    }

    public async void OnSearchButtonClicked(string fileName, string content)
    {
        SearchResults.Clear();
        var results = new ObservableCollection<SearchResult>(_fileSearcherService.SmartSearch(fileName, content));
        foreach (var result in results)
        {
            SearchResults.Add(result);
        }
    }
}