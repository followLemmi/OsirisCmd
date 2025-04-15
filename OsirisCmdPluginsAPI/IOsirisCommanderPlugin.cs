using Avalonia.Controls;

namespace OsirisCmdPluginsAPI;

public interface IOsirisCommanderPlugin
{
    
    string Name { get; }
    string Description { get; }
    string Author { get; }
    string Version { get; }
    string Help { get; }
    UserControl? SettingsTabContent { get; }
    
    void InitializeMainMenuItem(Menu mainMenuControl);

    void InitializeQuickAccessBar(Grid quickAccessBarControl);

    void InitializeExtendedToolbar(Grid extendedToolbarControl);

    void InitializeFileView(Grid fileViewControl);

    void InitializeTaskBar(Grid taskBarControl);
    
}
