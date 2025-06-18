using System.Collections.ObjectModel;

namespace OsirisCmd.SettingsManager;

public interface ISettings
{
    public ObservableCollection<SettingItem> Settings { get; }
    
}