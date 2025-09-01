using System;
using Application.Core.Services.FileSearcher;
using Application.Core.Services.Logger;
using Application.Core.Services.SettingsManager;
using Application.Services.FileSearcher;
using Application.Services.Logger;
using Application.Services.SettingsManager;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Services;

public static class MainServiceProvider
{
    public static IServiceProvider ServiceProvider { get; private set; } = null!;    
    
    public static void Build()
    {
        var services = new ServiceCollection();

        services.AddSingleton<ILoggerService, LoggerService>();
        services.AddSingleton<ISettingsProviderService, SettingsProviderService>();
        services.AddSingleton<IFileSearcherService, FileSearcherService>();
        
        ServiceProvider = services.BuildServiceProvider();
    }
}