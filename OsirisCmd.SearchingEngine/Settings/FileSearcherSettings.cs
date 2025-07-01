using System.Collections;
using System.Collections.ObjectModel;
using OsirisCmd.SettingsManager;

namespace OsirisCmd.SearchingEngine.Settings;


public class FileSearcherSettings : ISettings
{
    /// <summary>
    /// File Searcher settings structure and default values of settings
    /// </summary>
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
                        },
                        new () {
                            Name = "General",
                            Value = new List<string>()
                            {
                                "node_modules",
                                ".git",
                                ".svn",
                                ".hg",
                                "__pycache__",
                                ".gradle",
                                ".idea",
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
                        // Archives
                        ".zip",
                        ".rar",
                        ".7z",
                        ".tar",
                        ".gz",
                        ".bz2",
                        ".xz",
                        ".cab",
                        ".iso",
                        ".img",
                        ".dmg",
                        ".vhd",
                        ".vhdx",
                        ".jar",
                        ".war",

                        // Binary
                        ".exe",
                        ".dll",
                        ".bin",
                        ".msi",
                        ".sys",
                        ".ocx",
                        ".so",
                        ".deb",
                        ".rpm",
                        ".apk",
                        ".com",
                        ".out",
                        ".o",
                        ".a",

                        // Media
                        ".mp3",
                        ".wav",
                        ".flac",
                        ".aac",
                        ".ogg",
                        ".m4a",
                        ".mp4",
                        ".avi",
                        ".mkv",
                        ".mov",
                        ".webm",
                        ".wmv",
                        ".flv",

                        // Images
                        ".jpg",
                        ".jpeg",
                        ".png",
                        ".bmp",
                        ".gif",
                        ".tiff",
                        ".webp",
                        ".svg",
                        ".ico",
                        ".heic",
                        ".raw",
                        ".nef",
                        ".cr2",
                        ".arw",

                        // Modeling
                        ".dwg",
                        ".dxf",
                        ".stl",
                        ".obj",
                        ".3ds",
                        ".fbx",
                        ".blend"
                    }
                }
            }
        },
    ];

    public List<string> GetAllDirectoriesToSkip()
    {
        var result = new List<string>();

        var directoriesSettings = Settings.First(item => item.Name.Equals("DirectoriesSettings"));
        if (directoriesSettings != null)
        {
            var directoriesToSkip = ((List<SettingItem>)directoriesSettings.Value).First(item => item.Name.Equals("SkipDirectories"));
            ((List<SettingItem>) directoriesToSkip.Value).ForEach(item => 
            {
                ((List<string>)item.Value).ForEach(directory => result.Add(directory));
            });
            return result;
        }
        throw new Exception("Settings for directories not found");
    }


}