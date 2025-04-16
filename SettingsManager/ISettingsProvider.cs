using Avalonia.Controls;

namespace SettingsManager;

public interface ISettingsProvider
{
    string SettingsTabName { get; }
    
    UserControl? SettingsTabContent { get; }
}