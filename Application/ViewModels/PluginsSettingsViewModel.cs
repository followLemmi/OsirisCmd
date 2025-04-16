using System.Collections.ObjectModel;
using System.Linq;
using PluginsManager;
using SettingsManager;

namespace Application.ViewModels;

public class PluginsSettingsViewModel
{
    public ObservableCollection<Plugin> Plugins { get; }
    private SettingsProvider _settingsProvider = SettingsProvider.Instance;

    public PluginsSettingsViewModel()
    {
        Plugins = PluginManager.Instance.GetPlugins();
    }

    public void EnableDisablePlugin(Plugin plugin, bool enable)
    {
        _settingsProvider.ApplicationSettings.PluginSettings.Single(p => p.Name == plugin.Name).Enabled = enable;
    }
}