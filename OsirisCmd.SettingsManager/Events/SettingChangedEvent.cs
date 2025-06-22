using Serilog;

namespace OsirisCmd.SettingsManager.Events;

public static class SettingChangedEvent
{
    public delegate void SettingChangedDelegate(SettingItem settingItem);
    public static event SettingChangedDelegate? SettingChanged;

    public static void Invoke(SettingItem settingItem)
    {
        Log.Debug("Invoke Event --- SettingChangedEvent");
        SettingChanged!.Invoke(settingItem);
    }
}