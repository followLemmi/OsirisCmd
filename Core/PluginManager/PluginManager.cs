using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using Avalonia.Controls;
using OsirisCmdPluginInterface.core;

namespace OsirisCmd.core.PluginManager;

public class PluginManager
{

    private readonly string _pluginPath = "plugins/";
    private readonly ObservableCollection<Plugin> _plugins = [];

    private static readonly object _lock = new object();
    private static PluginManager? _instance;

    public static PluginManager Instance
    {
        get
        {
            if (_instance == null)
            {
                throw new InvalidOperationException("PluginManager has not been initialized.");
            }

            return _instance;
        }
    }

    public PluginManager(Window mainWindow)
    {
        _instance = this;
        LoadPlugins();
    }
    
    public static void Initialize(Window mainWindow)
    {
        lock (_lock)
        {
            if (_instance != null)
            {
                throw new InvalidOperationException("PluginManager has already been initialized.");
            }

            _instance = new PluginManager(mainWindow);
        }
    }
    
    public ObservableCollection<Plugin> GetPlugins()
    {
        return _plugins;
    }

    private String[] CollectPlugins()
    {
        return Directory.GetFiles(Path.GetFullPath(_pluginPath), "*.dll");
    }

    private void LoadPlugins()
    {
        String[] plugins = CollectPlugins();
        foreach (string plugin in plugins)
        {
            try
            {
                var assembly = Assembly.LoadFile(plugin);
                foreach (Type type in assembly.GetTypes())
                {
                    if (typeof(IOsirisCommanderPlugin).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                    {
                        IOsirisCommanderPlugin? pluginInstance = Activator.CreateInstance(type) as IOsirisCommanderPlugin;
                        if (pluginInstance != null) 
                        {
                            _plugins.Add(new Plugin(pluginInstance.Name, pluginInstance.Description, pluginInstance.Version, true));
                        }
                    }
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                Console.WriteLine($"Failed to load types from assembly {plugin}: {ex.Message}");
                foreach (var loaderException in ex.LoaderExceptions)
                {
                    Console.WriteLine(loaderException.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load assembly {plugin}: {ex.Message}");
            }
        }
    }
}