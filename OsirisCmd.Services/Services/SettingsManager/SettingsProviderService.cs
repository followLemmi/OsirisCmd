using System.Text.Json;
using Avalonia.Collections;
using Avalonia.Controls;
using OsirisCmd.Core.Converters;
using OsirisCmd.Core.Services.SettingsManager;
using OsirisCmd.Services.Events;

namespace OsirisCmd.Services.Services.SettingsManager;

public class SettingsProviderService : ISettingsProviderService
{
    private string SettingsFileName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/OsirisCmd/appdata/settings.json";

    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        WriteIndented = true,
        Converters = { new SettingsItemConverter() }
    };

    private AvaloniaDictionary<string, ISettings> SettingsSections { get; } = new();
    public AvaloniaDictionary<string, Func<UserControl>> UIComponents { get; } = new();
    private Dictionary<string, JsonElement> PendingSettings { get; } = new();

    public SettingsProviderService()
    {
        // TODO: Add link to logger service
        LoadSettings();
        
        SettingChangedEvent.SettingChanged += (changedSetting) =>
        {
            SaveSettings();
        };
    }

    private void LoadSettings()
    {
        if (!File.Exists(SettingsFileName))
        {
            SaveSettings();
        }

        var json = File.ReadAllText(SettingsFileName);
        var parsed = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json, _serializerOptions);

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

        File.WriteAllText(SettingsFileName, json);
    }
    
    public void RegisterUIComponent(string sectionName, Func<UserControl> uiComponent)
    {
        UIComponents[sectionName] = uiComponent;
    }

    public T? AttachSettings<T>() where T : class, ISettings, new()
    {
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
