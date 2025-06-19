using OsirisCmd.SearchingEngine.Settings;
using OsirisCmd.SettingsManager;

namespace OsirisCmd.SearchingEngine.ViewModels;

public class FileSearcherSettingsViewModel
{
    
    public FileSearcherSettings? Settings { get; set; }

    public FileSearcherSettingsViewModel()
    {
        Settings = SettingsProvider.Instance.AttachSettings<FileSearcherSettings>();
    }
    
}