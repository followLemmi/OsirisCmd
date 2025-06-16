using Avalonia.Controls;

namespace OsirisCmd.SettingsManager;

public class PluginsListSettingProvider(string settingName, UserControl? settingsTabContent) : ISettingsProvider
{
    public string SettingsTabName { get; } = settingName;
    public UserControl? SettingsTabContent { get; } = settingsTabContent;
}