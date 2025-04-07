using Avalonia.Controls;
using OsirisCmd.Components;

namespace OsirisCmd.Core.SettingsStorage;

public class PluginsListSettingProvider(string settingName, UserControl? settingsTabContent) : ISettingsProvider
{
    public string SettingsTabName { get; } = settingName;
    public UserControl? SettingsTabContent { get; } = settingsTabContent;
}