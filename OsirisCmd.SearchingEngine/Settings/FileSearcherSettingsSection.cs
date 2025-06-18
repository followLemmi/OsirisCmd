using OsirisCmd.SettingsManager;

namespace OsirisCmd.SearchingEngine;

public class FileSearchingSettingsSection : ISettingsSection {

    public string Name => "FileSearching";

    public ISettings Settings => new FileSearcherSettings();
}
