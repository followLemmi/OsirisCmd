using System.Collections.ObjectModel;
using Avalonia.Controls;
using OsirisCmd.Core.SettingsStorage;

namespace OsirisCmd.ViewModels;

public class SettingsWindowViewModel
{
    public ObservableCollection<ISetting> Settings { get; }
    public ObservableCollection<TabItem> Tabs { get; }

    public SettingsWindowViewModel()
    {
        Settings = new ObservableCollection<ISetting>()
        {
            new PluginSettingViewProvider()
        };
        Tabs = new ObservableCollection<TabItem>();
        Tabs.Add(new TabItem()
        {
            Header = "Settings",
            Content = "Test Content"
        });
        foreach (var settingTab in Settings)
        {
            Tabs.Add(new TabItem()
            {
                Header = settingTab.Name,
                Content = settingTab.Content
            });
        }
    }
    
}