using System.Runtime.CompilerServices;
using System.Text.Json;
using Avalonia.Collections;
using Avalonia.Controls;
using OsirisCmd.SettingsManager.Converters;
using OsirisCmd.SettingsManager.Events;
using Serilog;

namespace OsirisCmd.SettingsManager;

public class SettingsProvider
{
    private const string SettingsFileName = "settings.json";

    private static SettingsProvider? _instance;

    private readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions()
    {
        WriteIndented = true,
        Converters = { new SettingsItemConverter() }
    };

    private AvaloniaDictionary<string, ISettings> SettingsSections { get; } = new();
    public AvaloniaDictionary<string, Func<UserControl>> UIComponents { get; } = new();
    private Dictionary<string, JsonElement> PendingSettings { get; } = new();

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
        
        SettingChangedEvent.SettingChanged += (changedSetting) =>
        {
            Log.Debug("Settings changed {ChangedSetting}", changedSetting);
            SaveSettings();
        };
    }

    public static void Initialize()
    {
        Log.Debug("Initialize SettingsProvider");
        if (_instance != null)
        {
            Log.Error("SettingsProvider service has already been initialized.");
            throw new InvalidOperationException("Settings provider has already been initialized.");
        }
        
        _instance = new SettingsProvider();
    }

    private void LoadSettings()
    {
        Log.Debug("Loading settings");
        if (!File.Exists(SettingsFileName))
        {
            Log.Debug("Settings file does not exist. Creating new one.");
            SaveSettings();
        }

        var json = File.ReadAllText(SettingsFileName);
        Log.Debug("Settings file loaded: {Json}", json);
        var parsed = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json, _serializerOptions);
        Log.Debug("Settings parsed: {Parsed}", parsed);

        if (parsed == null) return;
        foreach (var (key, value) in parsed)
        {
            PendingSettings[key] = value;
        }
    }

    private void SaveSettings()
    {
        var jsonDictionary = new Dictionary<string, object>();
        foreach (var (key, settingsSection) in SettingsSections)
        {
            jsonDictionary[settingsSection.GetType().Name] = settingsSection;
        }

        var json = JsonSerializer.Serialize(jsonDictionary, _serializerOptions);

        Log.Debug("Saving settings: {Json}", json);
        File.WriteAllText(SettingsFileName, json);
    }

    public void RegisterUIComponent(string sectionName, Func<UserControl> uiComponent)
    {
        Log.Debug("Register UI for {SectionName}", sectionName);
        UIComponents[sectionName] = uiComponent;
    }

    public T? AttachSettings<T>() where T : class, ISettings, new()
    {
        Log.Debug("Attach settings for {SectionName}", typeof(T).Name);
        var sectionName = typeof(T).Name;
        if (SettingsSections.TryGetValue(sectionName, out var settingsSection))
        {
            return settingsSection as T;
        }

        if (PendingSettings.TryGetValue(sectionName, out var jsonElement))
        {
            var settings = jsonElement.Deserialize<T>(_serializerOptions) ?? new T();
            SettingsSections[sectionName] = settings;
            PendingSettings.Remove(sectionName);
            return SettingsSections[sectionName] as T;
        }

        var freshSettings = new T();
        SettingsSections[sectionName] = freshSettings;
        SaveSettings();
        return SettingsSections[sectionName] as T;
    }
}