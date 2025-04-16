using System.Collections.ObjectModel;
using Avalonia.Controls;
using SettingsManager;

namespace Application.ViewModels;

public class SettingsWindowViewModel
{
    public ObservableCollection<ISettingsProvider> Settings { get; }
    public ObservableCollection<TabItem> Tabs { get; }

    private readonly SettingsProvider _settingsProvider = SettingsProvider.Instance;

    public SettingsWindowViewModel()
    {
        Settings = [];
        foreach (var pluginSetting in _settingsProvider.PluginSettings)
        {
            Settings?.Add(pluginSetting);
        }
        Tabs = [];
        foreach (var settingTab in Settings)
        {
            Tabs.Add(new TabItem()
            {
                Header = settingTab.SettingsTabName,
                Content = settingTab.SettingsTabContent
            });
        }
    }
    
}