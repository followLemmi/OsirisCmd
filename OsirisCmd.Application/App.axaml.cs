using Application.Views;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using OsirisCmd.PluginsManager;
using OsirisCmd.SettingsManager;
using SearchingEngine;

namespace Application;

public partial class App : Avalonia.Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = new MainWindow();
            SettingsProvider.Initialize();
            PluginManager.Initialize();
            FileSearcher fileSearcher = new FileSearcher("./indexes");
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}