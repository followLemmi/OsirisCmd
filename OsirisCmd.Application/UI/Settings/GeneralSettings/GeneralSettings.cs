using System.Collections.ObjectModel;
using Application.Core.Models;
using Application.Core.Services.SettingsManager;

namespace Application.UI.Settings.GeneralSettings;

public class GeneralSettings : ISettings
{
    public ObservableCollection<SettingItem> Settings { get; set; } =
    [
    ];
}