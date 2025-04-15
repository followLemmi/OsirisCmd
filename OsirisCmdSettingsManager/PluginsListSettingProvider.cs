using Avalonia.Controls;

namespace OsirisCmdSettingsManager;

public class PluginsListSettingProvider(string settingName, UserControl? settingsTabContent) : ISettingsProvider
{
    public string SettingsTabName { get; } = settingName;
    public UserControl? SettingsTabContent { get; } = settingsTabContent;
}