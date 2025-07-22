using System.Collections.ObjectModel;
using System.Reflection;
using OsirisCmd.PluginsAPI;

namespace OsirisCmd.PluginsManager;

public class PluginManager
{
    private readonly string _pluginPath = Path.Combine(AppContext.BaseDirectory, "plugins/");
    private readonly ObservableCollection<Plugin> _plugins = [];

    private static readonly object Lock = new object();
    private static PluginManager? _instance;
    
    // private readonly SettingsProvider _settingsProvider = SettingsProvider.Instance;

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

    private string[] CollectPlugins()
    {
        return Directory.GetDirectories(_pluginPath);
    }

    private void LoadPlugins()
    {
        var pluginsFolders = CollectPlugins();
        foreach (var pluginFolder in pluginsFolders)
        {
            var pluginManifest = PluginManifest.ParsePluginManifest(pluginFolder + "/manifest.plug");
            try
            {
                // var pluginDllPath = Path.Combine(pluginFolder, pluginManifest.MainDllName);
                // var assembly = Assembly.LoadFile(pluginDllPath);
                // foreach (var type in assembly.GetTypes())
                // {
                //     if (!typeof(IOsirisCommanderPlugin).IsAssignableFrom(type) || type.IsInterface ||
                //         type.IsAbstract) continue;
                //     if (Activator.CreateInstance(type) is not IOsirisCommanderPlugin pluginInstance) continue;
                //     // var pluginSettings = _settingsProvider.ApplicationSettings.PluginSettings
                //     //     .FirstOrDefault(p => p.Name == pluginInstance.Name);
                //     var pluginObject = new Plugin(pluginInstance.Name, pluginInstance.Description, pluginInstance.Author,
                //         pluginInstance.Version, pluginInstance.SettingsTabContent, pluginSettings == null || pluginSettings.Enabled);
                //     if (pluginObject.IsEnabled)
                //     {
                //         _plugins.Add(pluginObject);
                //         if (pluginObject.SettingsTabContent != null)
                //         {
                //             // _settingsProvider.AddPluginSettings(pluginObject.Name, pluginObject.SettingsTabContent);
                //         }
                //     }
                //     // _settingsProvider.ApplicationSettings.PluginSettings.Add(new PluginSettings()
                //     // {
                //     //     Name = pluginObject.Name,
                //     //     Enabled = true
                //     // });
                // }
            }
            catch (ReflectionTypeLoadException ex)
            {
                Console.WriteLine($"Failed to load types from assembly {pluginManifest.MainDllName}: {ex.Message}");
                foreach (var loaderException in ex.LoaderExceptions)
                {
                    Console.WriteLine(loaderException.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load assembly {pluginManifest.MainDllName}: {ex.Message}");
            }
        }
    }
}