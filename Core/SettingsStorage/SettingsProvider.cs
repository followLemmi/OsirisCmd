using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using OsirisCmd.Core.SettingsStorage.Events;

namespace OsirisCmd.Core.SettingsStorage;

public class SettingsProvider
{
    private readonly string _settingsFileName = "settings.json";
    
    private static SettingsProvider? _instance;

    public ApplicationSettings ApplicationSettings { get; }

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
        ApplicationSettings = LoadSettings();
        
        SettingChangedEvent.SettingChanged += SettingChangedEventHandler;
    }

    private void SettingChangedEventHandler()
    {
        SaveSettings();
    }

    public static void Initialize()
    {
        if (_instance != null)
        {
            throw new InvalidOperationException("Settings provider has already been initialized.");
        }
        _instance = new SettingsProvider();
    }

    private ApplicationSettings LoadSettings() 
    {
        if (!File.Exists(_settingsFileName))
        {
            var settings = new ApplicationSettings();
            File.WriteAllText(_settingsFileName, JsonSerializer.Serialize(settings));
            return settings;
        }

        var json = File.ReadAllText(_settingsFileName);
        var applicationSettings = JsonSerializer.Deserialize<ApplicationSettings>(json);
        return applicationSettings ?? new ApplicationSettings();
    }

    private void SaveSettings()
    {
        File.WriteAllText(_settingsFileName, JsonSerializer.Serialize(ApplicationSettings));
    }
}