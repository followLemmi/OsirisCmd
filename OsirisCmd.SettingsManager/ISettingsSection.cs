using Avalonia.Controls;
using OsirisCmd.SettingsManager;

public interface ISettingsSection {

    string Name { get; }
    
    ISettings Settings { get; }
}
