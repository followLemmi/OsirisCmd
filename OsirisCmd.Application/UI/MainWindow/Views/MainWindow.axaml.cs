using Application.UI.FileSearcher;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace Application.UI.MainWindow.Views;

public partial class MainWindow : Window
{

    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.KeyModifiers == (KeyModifiers.Control | KeyModifiers.Shift) && e.Key == Key.F)
        {
            ShowFileSearcherWindow();
            e.Handled = true;
            return;
        }
        base.OnKeyDown(e);
    }
    
    private AvaloniaObject ShowFileSearcherWindow()
    {
        var fileSearcherWindow = new FileSearcherWindow();
        fileSearcherWindow.ShowDialog(this);
        return fileSearcherWindow;
    }
    
}
