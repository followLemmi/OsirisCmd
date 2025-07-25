﻿using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using OsirisCmd.Core.Models;
using OsirisCmd.Core.Services.SettingsManager;

namespace OsirisCmd.Services.Services.FileSearcher.Settings;

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
            Name = "PathToIndexes",
            Value = GetDefaultOsSpecificPathToIndexes()
        },
        new()
        {
        Name = "DrivesToIndex",
        Value = GetDefaultDrivesToIndex()
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
                                "/snap",
                                "/boot",
                                "/bin/X11",
                                "/usr/bin/X11",
                                "/var/lib",
                                "/dev"
                            }
                        },
                        new()
                        {
                            Name = "Windows",
                            Value = new List<string>()
                            {
                                "\\$Recycle.Bin",
                                "\\ProgrammData",
                            }
                        },
                        new()
                        {
                            Name = "MacOS",
                            Value = new List<string>()
                            {
                                "/System",
                                "/private",
                            }
                        },
                        new()
                        {
                            Name = "General",
                            Value = new List<string>()
                            {
                                "node_modules",
                                ".hg",
                                "__pycache__",
                                ".gradle",
                                ".idea",
                                ".git",
                                ".svn"
                            }
                        }
                    }
                },
            }
        },
        new()
        {
            Name = "FileSettings",
            Value = new List<SettingItem>()
            {
                new()
                {
                    Name = "ReadContentExtensions",
                    Value = new List<string>()
                    {
                        ".txt", ".md", ".rst", ".log", ".csv", ".tsv",

                        ".ini", ".conf", ".properties", ".env",

                        ".c", ".h", ".cpp", ".cc", ".cxx", ".hpp", ".hxx", ".cs", ".java", ".py",
                        ".js", ".ts", ".rb", ".go", ".rs", ".php", ".swift", ".kt", ".kts", ".dart",
                        ".m", ".mm", ".scala", ".hs", ".lua", ".pl", ".r", ".jl", ".groovy", ".clj",
                        ".sql", ".asm", ".s",

                        ".html", ".htm", ".xhtml", ".css", ".scss", ".sass", ".less", ".vue",
                        ".jsx", ".tsx",

                        ".sh", ".bash", ".zsh", ".fish", ".ps1", ".bat", ".cmd", ".make",
                        ".gradle", ".pom",

                        ".feature", ".spec.js", ".test.js", ".robot", ".doctest", ".story"
                    }
                },
                new()
                {
                    Name = "ReadContentFiles",
                    Value = new List<string>()
                    {
                        "Dockerfile",
                        "Makefile"
                    }
                }
            }
        },
    ];

    private static List<DriveToIndex>? GetDefaultDrivesToIndex()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return [];
        }

        return DriveInfo.GetDrives().Select(drive => new DriveToIndex() { Name = drive.Name, Enabled = true, })
            .ToList();
    }

    private static string GetDefaultOsSpecificPathToIndexes()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return "~/.config/osiris/indexes/";
        }
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\OsirisCmd\\indexes\\";
        }
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return "~/Library/Preferences/OsirisCmd/indexes/";
        }

        return "";
    }

    public bool IsFileIndexingEnabled()
    {
        var fileIndexingEnabled = Settings.First(item => item.Name.Equals("FileIndexingEnabled"));
        return (bool)fileIndexingEnabled.Value;
    }

    public List<DriveToIndex>? GetDrivesToIndex()
    {
        var drivesToIndex = Settings.First(item => item.Name.Equals("DrivesToIndex"));
        if (drivesToIndex == null) throw new Exception("Settings for drives to index not found");
        if (drivesToIndex.Value == null) throw new Exception("Settings for drives to index have no value");
        return drivesToIndex.Value as List<DriveToIndex>;
    }

    public List<string> GetAllDirectoriesToSkip()
    {
        var result = new List<string>();

        var directoriesSettings = Settings.First(item => item.Name.Equals("DirectoriesSettings"));
        if (directoriesSettings != null)
        {
            var directoriesToSkip =
                ((List<SettingItem>)directoriesSettings.Value).First(item => item.Name.Equals("SkipDirectories"));
            ((List<SettingItem>)directoriesToSkip.Value).ForEach(item =>
            {
                ((List<string>)item.Value).ForEach(directory => result.Add(directory));
            });
            return result;
        }

        throw new Exception("Settings for directories not found");
    }

    public List<string> GetReadContentExtensions()
    {
        var result = new List<string>();

        var fileSettings = Settings.First(item => item.Name.Equals("FileSettings"));
        if (fileSettings != null)
        {
            var fileNameOnly =
                ((List<SettingItem>)fileSettings.Value).First(item => item.Name.Equals("ReadContentExtensions"));
            ((List<string>)fileNameOnly.Value).ForEach(item => result.Add(item));
            return result;
        }

        throw new Exception("Settings for file name only not found");
    }

    public List<string> GetReadContentFiles()
    {
        var result = new List<string>();

        var fileSettings = Settings.First(item => item.Name.Equals("FileSettings"));
        if (fileSettings != null)
        {
            var fileNameOnly =
                ((List<SettingItem>)fileSettings.Value).First(item => item.Name.Equals("ReadContentFiles"));
            ((List<string>)fileNameOnly.Value).ForEach(item => result.Add(item));
            return result;
        }

        throw new Exception("Settings for file name only not found");
    }

    public string GetPathToIndexes()
    {
        var pathToIndexes = Settings.First(item => item.Name.Equals("PathToIndexes"));
        if (pathToIndexes == null) throw new Exception("Settings for path to indexes not found");
        if (pathToIndexes.Value == null) throw new Exception("Settings for path to indexes have no value");
        return (string)pathToIndexes.Value;
    }
}
