using Avalonia.Controls;
using Avalonia.Interactivity;

namespace OsirisCmd.Views;

public partial class MainMenuControl : UserControl
{
    // private PluginManager _pluginManager;
    public MainMenuControl()
    {
        InitializeComponent();
        
    }
    
    private void OpenSettingsHandler(object? sender, RoutedEventArgs e)
    {
        var settingsWindow = new SettingsWindow();
        settingsWindow.ShowDialog((Window) this.VisualRoot);
    }

    private void GroupRenameHandler(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void CloseApplicationHandler(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void AddTabLeftHandler(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void AddTabRightHandler(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void OpenApplicationSettingsJsonHandler(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void ImportSettingsHandler(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void ExportSettingsHandler(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void AboutApplicationHandler(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void WikiHandler(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void GitHubHandler(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void ReportBugHandler(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }

    private void SendSuggestionsHandler(object? sender, RoutedEventArgs e)
    {
        throw new System.NotImplementedException();
    }
}