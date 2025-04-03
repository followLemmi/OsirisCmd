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
    
    private void SettingsItem_OnClick(object? sender, RoutedEventArgs e)
    {
        var settingsWindow = new SettingsWindow();
        settingsWindow.ShowDialog((Window) this.VisualRoot);
    }
}