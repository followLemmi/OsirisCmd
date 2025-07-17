using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using OsirisCmd.SettingsManager;

namespace Application.Settings;

public class GeneralSettings : ISettings
{
    public ObservableCollection<SettingItem> Settings { get; set; } =
    [
        new()
        {
            Name = "PathToApplicationSettings",
            Value = GetDefaultOsSpecificPathToIndexes()
        }
    ];
    
    private static string GetDefaultOsSpecificPathToIndexes()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return "~/.config/osiris/";
        }
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\OsirisCmd\\";;
        }
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return "~/Library/Preferences/OsirisCmd/";
        }

        return "";
    }
    
    public string GetPathToIndexes()
    {
        var pathToIndexes = Settings.First(item => item.Name.Equals("PathToIndexes"));
        if (pathToIndexes == null) throw new Exception("Settings for path to indexes not found");
        if (pathToIndexes.Value == null) throw new Exception("Settings for path to indexes have no value");
        return (string)pathToIndexes.Value;
    }
}