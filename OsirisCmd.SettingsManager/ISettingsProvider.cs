using Avalonia.Controls;

namespace OsirisCmd.SettingsManager;

public interface ISettingsProvider
{
    string SettingsTabName { get; }
    
    UserControl? SettingsTabContent { get; }
}
