using System.Text.Json;
using Avalonia.Collections;
using OsirisCmd.Core.Converters;
using OsirisCmd.Core.Services.SettingsManager;
using OsirisCmd.Services.Events;
using OsirisCmd.Core.Services.Logger;

namespace OsirisCmd.Services.Services.SettingsManager;

public class SettingsProviderService : ISettingsProviderService
{
    private ILoggerService _logger;

    private string SettingsFileName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/OsirisCmd/appdata/settings.json";

    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        WriteIndented = true,
        Converters = { new SettingsItemConverter() }
    };

    private AvaloniaDictionary<string, ISettings> SettingsSections { get; } = new();
    private Dictionary<string, JsonElement> PendingSettings { get; } = new();

    public SettingsProviderService(ILoggerService logger)
    {
        _logger = logger;

        LoadSettings();
        
        SettingChangedEvent.SettingChanged += (changedSetting) =>
        {
            SaveSettings();
        };
    }

    private void LoadSettings()
    {
        _logger.LogDebug("Start loading application settings");
        if (!File.Exists(SettingsFileName))
        {
            _logger.LogDebug("Settings file not exist. Create default.");
            SaveSettings();
        }

        var json = File.ReadAllText(SettingsFileName);
        var parsed = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json, _serializerOptions);

        if (parsed == null) return;
        foreach (var (key, value) in parsed)
        {
            PendingSettings[key] = value;
        }
        _logger.LogDebug($"Loaded settings:\n{json}");
    }

    private void SaveSettings()
    {
        _logger.LogDebug("Save settings called");
        var jsonDictionary = new Dictionary<string, object>();
        foreach (var (key, settingsSection) in SettingsSections)
        {
            jsonDictionary[settingsSection.GetType().Name] = settingsSection;
        }

        var json = JsonSerializer.Serialize(jsonDictionary, _serializerOptions);

        File.WriteAllText(SettingsFileName, json);
    }

    public T? AttachSettings<T>() where T : class, ISettings, new()
    {
        var sectionName = typeof(T).Name;
        _logger.LogDebug($"SettingsAttach requested for {sectionName}");
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
