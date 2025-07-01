using System.Collections.ObjectModel;
using OsirisCmd.SettingsManager;

namespace OsirisCmd.SearchingEngine.Settings;

public class FileSearcherSettings : ISettings
{
    public ObservableCollection<SettingItem> Settings { get; set; } =
    [
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
                    Value = new List<SettingItem>()
                    {
                        new()
                        {
                            Name = "Linux",
                            Value = new List<string>()
                            {
                                "/proc",
                                "/sys",
                                "/run",
                                "/bin",
                                "/sbin",
                                "/tmp",
                                "/boot",
                                "/var/run",
                                "/var/lock",
                                "/var/tmp",
                                "/var/log",
                                "/var/spool",
                                "/var/cache",
                                "/lost+found",
                                "/home/user/.cache",
                                "/home/user/.thumbnails",
                            }
                        },
                        new ()
                        {
                            Name = "Windows",
                            Value = new List<string>()
                            {
                                "C:\\Windows",
                                "C:\\ProgramData",
                                "C:\\$Recycle.Bin",
                                "C:\\System Volume Information",
                            }
                        },
                        new ()
                        {
                            Name = "MacOS",
                            Value = new List<string>()
                            {
                                "/System",
                                "/private",
                                "/usr/local",
                                "/usr/share",
                                "/usr/lib",
                                "/usr/bin",
                                "/usr/sbin",
                                "/.Trash",
                                "/.Spotlight-V100",
                                "/.fseventsd",
                            }
                        }
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
            Name = "FileSettings",
            Value = new List<SettingItem>()
            {
                new()
                {
                    Name = "FileNameOnly",
                    Value = new List<string>()
                    {
                        ".exe",
                        ""
                    }
                }
            }
        },
    ];
}