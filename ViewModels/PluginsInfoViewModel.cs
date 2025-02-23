using System.Collections.Generic;
using System.Collections.ObjectModel;
using OsirisCmd.core.PluginManager;
using OsirisCmdPluginInterface.core;

namespace OsirisCmd.ViewModels;

public class PluginsInfoViewModel
{
    public ObservableCollection<Plugin> Plugins { get; }

    public PluginsInfoViewModel()
    {
        Plugins = PluginManager.Instance.GetPlugins();
    }
}