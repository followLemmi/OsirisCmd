using System.IO;
using Application.Core.Models;
using Application.Services.FileSearcher;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Application.UI.FileSearcher;

public partial class FileSearcherWindow : Window
{
    public FileSearcherWindow()
    {
        InitializeComponent();
    }

    private void SearchButtonOnClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is FileSearcherWindowViewModel viewModel && sender is Button button)
        {
            var fileNameTextBoxContent = FileNameSearch.Text;
            var fileContentBoxContent = FileContentSearch.Text;

            // Collect Options
            var isFileNameCaseSensitive = IsFileNameCaseSensitive.IsChecked;
            var isFileContentCaseSensitive = IsContentCaseSensitive.IsChecked;

            var searchOptions = new SearchOptions()
            {
                IsFileNameCaseSensitive = (bool) isFileNameCaseSensitive!,
                IsContentCaseSensitive = (bool) isFileContentCaseSensitive!,
            };
            
            viewModel.OnSearchButtonClicked(fileNameTextBoxContent, fileContentBoxContent, searchOptions);
        }
    }

    private void InputElement_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (sender is DataGrid dataGrid && dataGrid.SelectedItem is SearchResult searchResult)
        {
            var previewWindow = new FilePreviewWindow();
            var viewModel = new FilePreviewWindowViewModel();
            previewWindow.DataContext = viewModel;
            viewModel.FileContent = File.ReadAllText(searchResult.FilePath);
            viewModel.FileName = searchResult.FileName;
            viewModel.Extension = searchResult.Extension;
            previewWindow.Show();
        }
    }

    private void ExpandSearchOptionsPaneButtonHandler(object? sender, RoutedEventArgs e)
    {
        if (DataContext is FileSearcherWindowViewModel viewModel)
        {
            viewModel.IsSearchOptionsPaneOpened = !viewModel.IsSearchOptionsPaneOpened;
        }
    }
}
