using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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

    private PluginManager()
    {
        _instance = this;
        LoadPlugins();
        
    }
    
    public static void Initialize()
    {
        lock (Lock)
        {
            if (_instance != null)
            {
                throw new InvalidOperationException("PluginManager has already been initialized.");
            }

            _instance = new PluginManager();
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
        var plugins = CollectPlugins();
        foreach (var plugin in plugins)
        {
            try
            {
                var assembly = Assembly.LoadFile(plugin);
                foreach (var type in assembly.GetTypes())
                {
                    if (!typeof(IOsirisCommanderPlugin).IsAssignableFrom(type) || type.IsInterface ||
                        type.IsAbstract) continue;
                    if (Activator.CreateInstance(type) is not IOsirisCommanderPlugin pluginInstance) continue;
                    var pluginSettings = _settingsProvider.ApplicationSettings.PluginSettings
                        .FirstOrDefault(p => p.Name == pluginInstance.Name);
                    var pluginObject = new Plugin(pluginInstance.Name, pluginInstance.Description, pluginInstance.Author,
                        pluginInstance.Version, pluginInstance.SettingsTabContent, pluginSettings!.Enabled);
                    if (pluginObject.IsEnabled)
                    {
                        _plugins.Add(pluginObject);
                        if (pluginObject.SettingsTabContent != null)
                        {
                            _settingsProvider.AddPluginSettings(pluginObject.Name, pluginObject.SettingsTabContent);
                        }
                    }
                    _settingsProvider.ApplicationSettings.PluginSettings.Add(new PluginSettings()
                    {
                        Name = pluginObject.Name,
                        Enabled = true
                    });
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