using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using OsirisCmdPluginInterface.core;

namespace OsirisCmd.core.PluginManager;

public class PluginManager
{

    private String _pluginPath = "plugins/";

    private List<IOsirisCommanderPlugin> _plugins = [];

    public PluginManager()
    {

    }

    private String[] CollectPlugins()
    {
        return Directory.GetFiles(_pluginPath, "*.dll");
    }

    public void LoadPlugins()
    {
        String[] plugins = CollectPlugins();
        foreach (String plugin in plugins)
        {
            var assembly = Assembly.LoadFile(plugin);
            var pluginType = assembly.GetType("OsirisCommander");
        }
    }
}