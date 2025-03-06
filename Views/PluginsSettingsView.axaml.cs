using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;
using OsirisCmd.core.PluginManager;
using OsirisCmd.ViewModels;

namespace OsirisCmd.Views;

public partial class PluginsSettingsView : UserControl
{
    public PluginsSettingsView()
    {
        InitializeComponent();
    }

    private void ToggleButton_OnIsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        var plugin = (sender as ToggleSwitch)?.DataContext as Plugin;
        // Debug.WriteLine($"checked {(sender as ToggleSwitch)?.IsChecked} - source {plugin}");
        (DataContext as PluginsSettingsViewModel).EnableDisablePlugin(plugin, (bool)(sender as ToggleSwitch)?.IsChecked);
    }
}