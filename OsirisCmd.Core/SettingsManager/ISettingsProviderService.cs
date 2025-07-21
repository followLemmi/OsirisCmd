using Avalonia.Collections;
using Avalonia.Controls;
using OsirisCmd.SettingsManager;

namespace OsirisCmd.Core.SettingsManager;

public interface ISettingsProviderService
{
    AvaloniaDictionary<string, Func<UserControl>> UIComponents { get; }

    void RegisterUIComponent(string sectionName, Func<UserControl> uiComponent);

    T? AttachSettings<T>() where T : class, ISettings, new();

}
