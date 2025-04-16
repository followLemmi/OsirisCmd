using System.ComponentModel;
using SettingsManager.Events;

namespace SettingsManager;

public class ApplicationSettings
{
    public BindingList<PluginSettings> PluginSettings { get; }

    public ApplicationSettings()
    {
        PluginSettings = [];
        PluginSettings.ListChanged += (_, _) => SettingChangedEvent.Invoke();
    }
    
    
}