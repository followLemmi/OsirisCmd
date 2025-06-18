using OsirisCmd.SettingsManager;

namespace OsirisCmd.SearchingEngine;

public class FileSearcherSettings : ISettings
{
    public bool FileSearcherEnabled { get; set; }
    
    public List<string> ExtensionExclude = [];

}
