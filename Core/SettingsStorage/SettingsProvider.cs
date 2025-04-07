using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using Avalonia.Controls;
using OsirisCmd.Core.SettingsStorage.Events;

namespace OsirisCmd.Core.SettingsStorage;

public class SettingsProvider
{
    private const string SettingsFileName = "settings.json";

    private static SettingsProvider? _instance;

    public ObservableCollection<ISettingsProvider> PluginSettings { get; }
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
    
    public void AddPluginSettings(string settingName, UserControl? settingsTabContent)
    {
        PluginSettings.Add(new PluginsListSettingProvider(settingName, settingsTabContent));
    }

    private static ApplicationSettings LoadSettings() 
    {
        if (!File.Exists(SettingsFileName))
        {
            var settings = new ApplicationSettings();
            File.WriteAllText(SettingsFileName, JsonSerializer.Serialize(settings));
            return settings;
        }

        var json = File.ReadAllText(SettingsFileName);
        var applicationSettings = JsonSerializer.Deserialize<ApplicationSettings>(json);
        return applicationSettings ?? new ApplicationSettings();
    }

    private void SaveSettings()
    {
        File.WriteAllText(SettingsFileName, JsonSerializer.Serialize(ApplicationSettings));
    }
}