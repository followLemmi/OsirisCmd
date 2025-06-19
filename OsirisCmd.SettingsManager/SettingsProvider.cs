using System.Text.Json;
using Avalonia.Collections;
using Avalonia.Controls;
using SettingsManager.Events;

namespace OsirisCmd.SettingsManager;

public class SettingsProvider
{
    private const string SettingsFileName = "settings.json";

    private static SettingsProvider? _instance;

    public AvaloniaDictionary<string, ISettingsSection> SettingsSections { get; } = new();
    public AvaloniaDictionary<string, UserControl> UIComponents { get; } = new();
    private Dictionary<string, JsonElement> _pendingSettings { get; } = new();

    public static SettingsProvider Instance
    {
        get
        {
            if (_instance == null)
            {
                throw new InvalidOperationException("Settings provider has not been initialized.");
            }
            return _instance;
        }
    }

    private SettingsProvider()
    {
        _instance = this;
        LoadSettings();
        
        SettingChangedEvent.SettingChanged += (SaveSettings);
    }

    public static void Initialize()
    {
        if (_instance != null)
        {
            throw new InvalidOperationException("Settings provider has already been initialized.");
        }
        _instance = new SettingsProvider();
        
    }

    private void LoadSettings() 
    {
        if (!File.Exists(SettingsFileName))
        {
            SaveSettings();
        }
        var json = File.ReadAllText(SettingsFileName);
        var parsed = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

        if (parsed == null) return;
        foreach (var (key, value) in parsed)
        {
            _pendingSettings[key] = value;
        }
    }

    private void SaveSettings()
    {
        var jsonDictionary = new Dictionary<string, object>();
        foreach (var (key, settingsSection) in SettingsSections)
        {
            jsonDictionary[settingsSection.Name] = settingsSection.Settings;
        }

        var json = JsonSerializer.Serialize(jsonDictionary, new JsonSerializerOptions()
        {
            WriteIndented = true
        });
        
        File.WriteAllText(SettingsFileName, json);
    }

    public void RegisterUI(string sectionName, UserControl uiComponent) 
    {
        UIComponents[sectionName] = uiComponent;
    }

    public UserControl? GetUI(string sectionName)
    {
        return UIComponents[sectionName];
    }

    public T? AttachSettings<T>(string sectionName) where T : class, ISettingsSection, new()
    {
        if (SettingsSections.TryGetValue(sectionName, out var settingsSection))
        {
            return settingsSection as T;
        }

        if (_pendingSettings.TryGetValue(sectionName, out var jsonElement))
        {
            var settings = jsonElement.Deserialize<T>() ?? new T();
            SettingsSections[sectionName] = settings;
            _pendingSettings.Remove(sectionName);
            return SettingsSections[sectionName] as T;
        }
        
        var freshSettings = new T();
        SettingsSections[sectionName] = freshSettings;
        SaveSettings();
        return SettingsSections[sectionName] as T;
    }
}
