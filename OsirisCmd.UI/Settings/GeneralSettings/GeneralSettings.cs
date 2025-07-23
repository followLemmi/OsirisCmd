using System.Collections.ObjectModel;
using OsirisCmd.Core.Models;
using OsirisCmd.Core.Services.SettingsManager;

namespace Application.Settings;

public class GeneralSettings : ISettings
{
    public ObservableCollection<SettingItem> Settings { get; set; } =
    [
    ];
}