using System.Collections.ObjectModel;
using Avalonia.Controls;
using OsirisCmd.Core.SettingsStorage;

namespace OsirisCmd.ViewModels;

public class SettingsWindowViewModel
{
    public ObservableCollection<ISettingsProvider> Settings { get; }
    public ObservableCollection<TabItem> Tabs { get; }

    public SettingsWindowViewModel()
    {
        Tabs = new ObservableCollection<TabItem>();
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