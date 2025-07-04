﻿namespace SettingsManager.Events;

public static class SettingChangedEvent
{
    public delegate void SettingChangedHandler();
    public static event SettingChangedHandler? SettingChanged;

    public static void Invoke()
    {
        SettingChanged!.Invoke();
    }
}