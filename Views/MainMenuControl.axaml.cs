using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using OsirisCmd.core.PluginManager;

namespace OsirisCmd.Views;

public partial class MainMenuControl : UserControl
{
    // private PluginManager _pluginManager;
    public MainMenuControl()
    {
        InitializeComponent();
        
    }

    private void PluginsInfoClickHandler(object? sender, RoutedEventArgs e)
    {
        var pluginInfoWindow = new PluginsSettingsView();
        // pluginInfoWindow.ShowDialog((Window) this.VisualRoot);
    }

    private void SettingsItem_OnClick(object? sender, RoutedEventArgs e)
    {
        var settingsWindow = new SettingsWindow();
        settingsWindow.ShowDialog((Window) this.VisualRoot);
    }
}