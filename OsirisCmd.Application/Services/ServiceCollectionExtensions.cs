using Application.Core.Services.FileSearcher;
using Application.Core.Services.Logger;
using Application.Core.Services.SettingsManager;
using Application.Services.FileSearcher;
using Application.Services.Logger;
using Application.Services.SettingsManager;
using Microsoft.Extensions.DependencyInjection;

namespace OsirisCmd.Services;

public static class ServiceCollectionExtensions
{

    public static IServiceCollection AddOsirisCmdServices(this IServiceCollection services)
    {
        return services;
    }
    
}
