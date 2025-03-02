using System.IO;
using System.Text.Json;

namespace OsirisCmd.Core.SettingsStorage;

public class SettingsProvider
{
    private readonly string _settingsFileName = "settings.json";

    public ApplicationSettings Settings { get; }
    
    public SettingsProvider()
    {
        Settings = LoadSettings();
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
}