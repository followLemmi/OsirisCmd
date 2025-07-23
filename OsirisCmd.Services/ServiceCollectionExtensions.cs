using Microsoft.Extensions.DependencyInjection;
using OsirisCmd.Core.Services.FileSearcher;
using OsirisCmd.Core.Services.Logger;
using OsirisCmd.Core.Services.SettingsManager;
using OsirisCmd.Services.Services.FileSearcher;
using OsirisCmd.Services.Services.Logger;
using OsirisCmd.Services.Services.SettingsManager;

namespace OsirisCmd.Services;

public static class ServiceCollectionExtensions
{

    public static IServiceCollection AddOsirisCmdServices(this IServiceCollection services)
    {
        services.AddSingleton<ILoggerService, LoggerService>();
        services.AddSingleton<ISettingsProviderService, SettingsProviderService>();
        services.AddSingleton<IFileSearcherService, FileSearcherService>();
        return services;
    }
    
}