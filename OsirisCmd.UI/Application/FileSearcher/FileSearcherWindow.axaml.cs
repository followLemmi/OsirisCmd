using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using OsirisCmd.Core.Models;

namespace OsirisCmd.UI.Application.FileSearcher;

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
            
            viewModel.OnSearchButtonClicked(fileNameTextBoxContent);
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
}
