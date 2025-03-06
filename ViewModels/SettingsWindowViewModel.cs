using System.Collections.ObjectModel;
using Avalonia.Controls;
using OsirisCmd.Core.SettingsStorage;

namespace OsirisCmd.ViewModels;

public class SettingsWindowViewModel
{
    public ObservableCollection<ISetting> Settings { get; }

    public SettingsWindowViewModel()
    {
        Settings = new ObservableCollection<ISetting>()
        {
            new PluginSettingViewProvider()
        };
    }
    
}