using System.IO;
using Application.Components;
using Application.Views;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using OsirisCmd.Logger;
using OsirisCmd.DI;
using OsirisCmd.SearchingEngine;
using OsirisCmd.SettingsManager;

namespace Application;

public partial class App : Avalonia.Application
{
    
    public override void Initialize()
    {

        // Initialize services
        DIContainer.Initialize(ConfigureServices);
        
        // Create Startup services
        var settingsProvider = ServiceLocator.GetService<ISettingsProviderService>();
        settingsProvider.RegisterUIComponent("General", () => new GeneralSettingsComponent());
        
        ServiceLocator.GetService<IFileSearcherService>();

        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = new MainWindow();
            desktop.MainWindow = mainWindow;

        }

        base.OnFrameworkInitializationCompleted();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<ILoggerService, LoggerService>();
        services.AddSingleton<ISettingsProviderService, SettingsProviderService>();
        services.AddSingleton<IFileSearcherService>(provider =>
        {
            var settingsProvider = provider.GetService<ISettingsProviderService>();
            return new FileSearcherService(settingsProvider);
        });
    }

}
