using Application.Views;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using OsirisCmd.DI;
using OsirisCmd.SearchingEngine;
using OsirisCmd.SettingsManager;

namespace Application;

public partial class App : Avalonia.Application
{
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        
        DIContainer.Initialize(ConfigureServices);
        ServiceLocator.GetService<IFileSearcherService>();
    }

    private void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<ISettingsProviderService, SettingsProviderService>();
        services.AddSingleton<IFileSearcherService>(provider =>
        {
            var settingsProvider = provider.GetService<ISettingsProviderService>();
            return new FileSearcherService(settingsProvider);
        });
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
    
}