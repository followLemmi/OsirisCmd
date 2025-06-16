using Avalonia.Controls;
using OsirisCmd.SettingsManager;

public interface ISettingsSection {

    string SectionName { get; }
    
    ISettings GetSettings { get; }
}
