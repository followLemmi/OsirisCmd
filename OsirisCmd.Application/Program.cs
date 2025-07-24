using System;
using Avalonia;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using OsirisCmd.Core.Services.Logger;
using OsirisCmd.Services;
using OsirisCmd.UI;

namespace Application;

class Program
{
    private static ILoggerService _logger;

    [STAThread]
    public static void Main(string[] args)
    {
        MainServiceProvider.Build();
        
        UIServiceProviderAdapter.InjectMainServiceProvider(MainServiceProvider.ServiceProvider);
        
        _logger = MainServiceProvider.ServiceProvider.GetRequiredService<ILoggerService>();
        try
        {
            CreateApplicationFolders();

            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception e)
        {
            _logger.LogFatal("Fatal exception on start application", e);
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    private static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
    }


    private static void CreateApplicationFolders()
    {
        _logger.LogDebug("Create startup folders");
        var applicationLocalAppDataPath =
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/OsirisCmd";
        if (!Directory.Exists(applicationLocalAppDataPath))
        {
            _logger.LogDebug($"Create application local app data folder {applicationLocalAppDataPath}");
            Directory.CreateDirectory(applicationLocalAppDataPath);
        }

        if (!Directory.Exists(applicationLocalAppDataPath + "/appdata"))
        {
            _logger.LogDebug($"Create application local app data folder {applicationLocalAppDataPath}/appdata");
            Directory.CreateDirectory(applicationLocalAppDataPath + "/appdata");
        }
    }
}