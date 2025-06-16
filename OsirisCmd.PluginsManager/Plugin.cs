using Avalonia.Controls;

namespace OsirisCmd.PluginsManager;

public class Plugin(string name, string description, string author, string version, UserControl? settingsTabContent, bool isEnabled)
{
    public string Name { get; } = name;
    public string Description { get; } = description;
    public string Author { get; } = author;
    public string Version { get; } = version;
    public UserControl? SettingsTabContent { get; } = settingsTabContent;
    public bool IsEnabled { get; } = isEnabled;
    
}