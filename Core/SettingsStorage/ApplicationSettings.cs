using System.Collections.Generic;
using System.ComponentModel;
using OsirisCmd.Core.SettingsStorage.Events;

namespace OsirisCmd.Core.SettingsStorage;

public class ApplicationSettings
{
    public BindingList<PluginSettings> PluginSettings { get; }

    public ApplicationSettings()
    {
        PluginSettings = [];
        PluginSettings.ListChanged += (sender, args) => SettingChangedEvent.Invoke();
    }
    
    
}