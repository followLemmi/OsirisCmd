using System.Collections.ObjectModel;
using OsirisCmd.SettingsManager;

namespace OsirisCmd.SearchingEngine.Settings;

public class FileSearcherSettings : ISettings
{

    public ObservableCollection<SettingItem> Settings { get; set; } = [
        new()
        {
            Name = "FileIndexingEnabled",
            Value = false
        },
        new()
        {
            Name = "DirectoriesSettings",
            Value = new List<SettingItem>()
            {
                new()
                {
                    Name = "SkipDirectories",
                    Value = new List<string>()
                    {
                        "/proc",
                        "/sys",
                        "/run",
                        "/var/run",
                        "/var/lock",
                        "/var/tmp",
                        "/tmp",
                        "/var/log",
                        "/var/spool",
                        "/var/cache",
                        "/lost+found",
                        "/boot",
                        "/home/user/.cache",
                        "/home/user/.thumbnails",
                    }
                },
                new()
                {
                    Name = "FileNameOnly",
                    Value = new List<string>()
                    {
                        "/dev"
                    }
                }
            }
        },
        new()
        {
            Name = "ExcludedExtensions",
            Value = new List<string>()
            {
                
            }
        }
    ];
    
}
