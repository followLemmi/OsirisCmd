using System.Collections.ObjectModel;
using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using OsirisCmd.Core.Services.SettingsManager;
using OsirisCmd.UI;

namespace Application.ViewModels;

public class SettingsWindowViewModel
{
    public ObservableCollection<TabItem> Tabs { get; }

    public SettingsWindowViewModel()
    {
        var settingsProviderService = UIServiceProviderAdapter.ServiceProvider.GetRequiredService<ISettingsProviderService>();
        Tabs = [];
        foreach (var settingTab in settingsProviderService.UIComponents)
        {
            Tabs.Add(new TabItem()
            {
                Header = settingTab.Key,
                Content = settingTab.Value()
            });
        }
    }
    
}
