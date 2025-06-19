using System.Collections.ObjectModel;
using OsirisCmd.SettingsManager;

namespace OsirisCmd.SearchingEngine.Settings;

public class FileSearcherSettings : ISettings
{

    public ObservableCollection<SettingItem> Settings { get; set; } = [
        new()
        {
            Name = "File Indexing Enabled",
            Value = false
        },
        new()
        {
            Name = "Excluded Directories",
            Value = new List<string>()
            {
                "Windows",
                "Program Files",
            }
        }
    ];
    
}
