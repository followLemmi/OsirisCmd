using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using Avalonia.Controls;
using OsirisCmd.Core.SettingsStorage;
using OsirisCmdPluginInterface.core;

namespace OsirisCmd.core.PluginManager;

public class PluginManager
{
    private readonly string _pluginPath = Path.Combine(AppContext.BaseDirectory, "plugins/");
    private readonly ObservableCollection<Plugin> _plugins = [];

    private static readonly object Lock = new object();
    private static PluginManager? _instance;
    
    private SettingsProvider _settingsProvider = SettingsProvider.Instance;

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

    private PluginManager(Window mainWindow)
    {
        _instance = this;
        LoadPlugins();
    }
    
    public static void Initialize(Window mainWindow)
    {
        lock (Lock)
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
        return Directory.GetFiles(_pluginPath, "*.dll");
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
                            var pluginObject = new Plugin(pluginInstance.Name, pluginInstance.Description,
                                pluginInstance.Version, true);
                            _plugins.Add(pluginObject);
                            _settingsProvider.ApplicationSettings.PluginSettings.Add(new PluginSettings()
                            {
                                Name = pluginObject.Name,
                                Enabled = true
                            });
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