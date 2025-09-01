using Application.Core.Services.FileSearcher;
using Application.Services;
using Application.UI;
using Application.UI.MainWindow.Views;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;

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
            desktop.MainWindow = mainWindow;

            MainServiceProvider.ServiceProvider.GetRequiredService<IFileSearcherService>();
        }

        base.OnFrameworkInitializationCompleted();
    }

}
