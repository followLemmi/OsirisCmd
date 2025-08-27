using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

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
}
