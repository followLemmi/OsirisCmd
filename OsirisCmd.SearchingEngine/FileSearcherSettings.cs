using Avalonia.Controls;
using OsirisCmd.SettingsManager;

namespace SearchingEngine;

public class FileSearchingSettingsSection : ISettingsSection {

    public string SectionName => "FileSearching";

    public ISettings GetSettings => new FileSearchingSettings();
}
