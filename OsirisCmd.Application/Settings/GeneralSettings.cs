using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using OsirisCmd.SettingsManager;

namespace Application.Settings;

public class GeneralSettings : ISettings
{
    public ObservableCollection<SettingItem> Settings { get; set; } =
    [
    ];
}