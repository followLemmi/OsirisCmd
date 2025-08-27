using System.Collections.ObjectModel;
using Avalonia.Controls;

namespace OsirisCmd.UI.Application.MainWindow.ViewModels;

public class SettingsWindowViewModel
{
    public ObservableCollection<TabItem> Tabs { get; }

    public SettingsWindowViewModel()
    {
        // TODO: Rewrite to new settings UI
        // var settingsProviderService = UIServiceProviderAdapter.ServiceProvider.GetRequiredService<ISettingsProviderService>();
        // Tabs = [];
        // foreach (var settingTab in settingsProviderService.UIComponents)
        // {
        //     Tabs.Add(new TabItem()
        //     {
        //         Header = settingTab.Key,
        //         Content = settingTab.Value()
        //     });
        // }
    }
    
}
