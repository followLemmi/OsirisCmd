using Avalonia.Collections;
using Avalonia.Controls;

namespace OsirisCmd.SettingsManager;

public interface ISettingsProviderService
{
    AvaloniaDictionary<string, Func<UserControl>> UIComponents { get; }

    void RegisterUIComponent(string sectionName, Func<UserControl> uiComponent);

    T? AttachSettings<T>() where T : class, ISettings, new();

}