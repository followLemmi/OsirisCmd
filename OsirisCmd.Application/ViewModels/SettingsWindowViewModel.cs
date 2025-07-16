using System.Collections.ObjectModel;
using Avalonia.Collections;
using Avalonia.Controls;
using OsirisCmd.DI;
using OsirisCmd.SettingsManager;

namespace Application.ViewModels;

public class SettingsWindowViewModel
{
    public ObservableCollection<TabItem> Tabs { get; }

    public SettingsWindowViewModel()
    {
        var settingsProvider = ServiceLocator.GetService<ISettingsProviderService>();
        
        Tabs = [];
        foreach (var settingTab in settingsProvider.UIComponents)
        {
            Tabs.Add(new TabItem()
            {
                Header = settingTab.Key,
                Content = settingTab.Value()
            });
        }
    }
    
}
