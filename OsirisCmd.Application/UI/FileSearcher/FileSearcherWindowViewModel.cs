using System.Collections.ObjectModel;
using Application.Core.Models;
using Application.Core.Services.FileSearcher;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Avalonia.Controls;
using Application.Services.FileSearcher;

namespace Application.UI.FileSearcher;

public class FileSearcherWindowViewModel : ViewModelBase
{
    private IFileSearcherService _fileSearcherService;
    private bool _isSearchOptionsPaneOpened = false;

    public bool IsSearchOptionsPaneOpened
    {
        get => _isSearchOptionsPaneOpened;
        set => SetProperty(ref _isSearchOptionsPaneOpened, value);
    }
    
    public ObservableCollection<SearchResult> SearchResults { get; set; } = [];

    public FileSearcherWindowViewModel()
    {
        if (!Design.IsDesignMode)
        {
            _fileSearcherService = MainServiceProvider.ServiceProvider.GetRequiredService<IFileSearcherService>();
        }
    }

    public async void OnSearchButtonClicked(string fileName, string content, SearchOptions searchOptions)
    {
        SearchResults.Clear();
        var results = new ObservableCollection<SearchResult>(_fileSearcherService.SmartSearch(fileName, content, searchOptions));
        foreach (var result in results)
        {
            SearchResults.Add(result);
        }
    }
}
