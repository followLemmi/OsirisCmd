using System.ComponentModel;
using SettingsManager.Events;

namespace OsirisCmd.SettingsManager;

public class ApplicationSettings
{
    public BindingList<PluginSettings> PluginSettings { get; }

    public ApplicationSettings()
    {
        PluginSettings = [];
        PluginSettings.ListChanged += (_, _) => SettingChangedEvent.Invoke();
    }
    
    
}