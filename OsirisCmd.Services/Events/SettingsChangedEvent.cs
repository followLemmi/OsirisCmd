namespace OsirisCmd.Services.Events;

public static class SettingChangedEvent
{
    public delegate void SettingChangedDelegate(object? settingItem);
    public static event SettingChangedDelegate? SettingChanged;

    public static void Invoke(object? settingItem)
    {
        SettingChanged!.Invoke(settingItem);
    }
}
