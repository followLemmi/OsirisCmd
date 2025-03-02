using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;
using OsirisCmd.core.PluginManager;

namespace OsirisCmd.Views;

public partial class PluginsInfoWindow : Window
{
    public PluginsInfoWindow()
    {
        InitializeComponent();
    }

    private void ToggleButton_OnIsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        var plugin = (sender as ToggleSwitch)?.DataContext as Plugin;
        Debug.WriteLine($"checked {(sender as ToggleSwitch)?.IsChecked} - source {plugin}");
    }
}