using System.ComponentModel;
using OsirisCmdSettingsManager.Events;

namespace OsirisCmdSettingsManager;

public class ApplicationSettings
{
    public BindingList<PluginSettings> PluginSettings { get; }

    public ApplicationSettings()
    {
        PluginSettings = [];
        PluginSettings.ListChanged += (_, _) => SettingChangedEvent.Invoke();
    }
    
    
}