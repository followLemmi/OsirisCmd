using Avalonia.Controls;
using OsirisCmd.Views;
using PluginsSettingsComponent = OsirisCmd.Components.PluginsSettingsComponent;

namespace OsirisCmd.Core.SettingsStorage;

public class PluginSettingViewProvider : ISetting
{
    public string Name { get; } = "Plugins";
    public UserControl Content { get; } = new PluginsSettingsComponent();
}