using System.Collections.ObjectModel;
using Avalonia.Collections;
using Avalonia.Controls;
using OsirisCmd.SettingsManager;

namespace Application.ViewModels;

public class SettingsWindowViewModel
{
    public ObservableCollection<TabItem> Tabs { get; }

    private readonly SettingsProvider _settingsProvider = SettingsProvider.Instance;

    public SettingsWindowViewModel()
    {
        Tabs = [];
        foreach (var settingTab in _settingsProvider.UIComponents)
        {
            Tabs.Add(new TabItem()
            {
                Header = settingTab.Key,
                Content = settingTab.Value()
            });
        }
    }
    
}
