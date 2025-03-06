using Avalonia.Controls;

namespace OsirisCmd.Core.SettingsStorage;

public interface ISetting
{
    string Name { get; }

    UserControl Content { get; }
}