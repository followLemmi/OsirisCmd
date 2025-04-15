using Avalonia.Controls;

namespace OsirisCmdSettingsManager;

public interface ISettingsProvider
{
    string SettingsTabName { get; }
    
    UserControl? SettingsTabContent { get; }
}