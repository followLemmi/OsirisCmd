using Avalonia.Controls;
using Avalonia.Interactivity;
using OsirisCmd.core.PluginManager;
using OsirisCmd.ViewModels;

namespace OsirisCmd.Components;

public partial class PluginsSettingsComponent : UserControl
{
    public PluginsSettingsComponent()
    {
        InitializeComponent();
    }

    private void ToggleButton_OnIsCheckedChanged(object? sender, RoutedEventArgs e)
    {
        var plugin = (sender as ToggleSwitch)?.DataContext as Plugin;
        var dataContext = DataContext as PluginsSettingsViewModel;
        dataContext?.EnableDisablePlugin(plugin!, (bool)(sender as ToggleSwitch)?.IsChecked!);

    }
}