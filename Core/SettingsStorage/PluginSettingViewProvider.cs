using Avalonia.Controls;
using OsirisCmd.Views;

namespace OsirisCmd.Core.SettingsStorage;

public class PluginSettingViewProvider : ISetting
{
    public string Name { get; } = "Plugins";
    public UserControl Content { get; } = new PluginsSettingsView();
}