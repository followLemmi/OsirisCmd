using System.Collections.ObjectModel;
using OsirisCmd.Core.Models;

namespace OsirisCmd.Core.Services.SettingsManager;

public interface ISettings
{
    public ObservableCollection<SettingItem> Settings { get; }
    
}