using Serilog;

namespace OsirisCmd.SettingsManager.Events;

public static class SettingChangedEvent
{
    public delegate void SettingChangedDelegate(object? settingItem);
    public static event SettingChangedDelegate? SettingChanged;

    public static void Invoke(object? settingItem)
    {
        Log.Debug("Invoke Event --- SettingChangedEvent");
        SettingChanged!.Invoke(settingItem);
    }
}
