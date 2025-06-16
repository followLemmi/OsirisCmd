using Avalonia.Controls;
using OsirisCmd.SearchingEngine.Components;

namespace SearchingEngine;

public class FileSearchingSettings : ISettingsSection {

    public string SectionName => "File Searching";

    public UserControl SettingsTabContent => new FileSearcherSettingsComponent();

}
