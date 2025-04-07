using Avalonia.Controls;

namespace OsirisCmd.Core.SettingsStorage;

public interface ISettingsProvider
{
    string SettingsTabName { get; }
    
    UserControl? SettingsTabContent { get; }
}