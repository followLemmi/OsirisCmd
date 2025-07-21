using System;
using Avalonia;
using System.IO;

namespace Application;

class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        CreateApplicationFolders();

        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();

    private static void CreateApplicationFolders()
    {
        var applicationLocalAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/OsirisCmd";
        if (!Directory.Exists(applicationLocalAppDataPath)) {
            Directory.CreateDirectory(applicationLocalAppDataPath);
        }
        if (!Directory.Exists(applicationLocalAppDataPath + "/appdata"))
        {
            Directory.CreateDirectory(applicationLocalAppDataPath + "/appdata");
        }
    }

}
