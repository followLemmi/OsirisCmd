using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace OsirisCmd.UI.Application.FileSearcher;

public partial class FileSearcherWindow : Window
{
    public FileSearcherWindow()
    {
        InitializeComponent();
    }

    private void TextBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (DataContext is FileSearcherWindowViewModel viewModel && sender is TextBox textBox)
        {
            viewModel.OnSearchTextChanged(textBox.Text ?? string.Empty);
        }
    }
}
