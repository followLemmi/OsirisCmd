﻿using Application.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;
using OsirisCmd.PluginsManager;

namespace Application.Components;

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
