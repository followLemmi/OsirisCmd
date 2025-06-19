using OsirisCmd.SettingsManager;

namespace OsirisCmd.SearchingEngine.Settings;

public class FileSearchingSettingsSection : ISettingsSection {

    public string Name => "FileSearching";

    public ISettings Settings { get; set; }
    
    public FileSearchingSettingsSection()
    {
        Settings = new FileSearcherSettings();
    }
}
