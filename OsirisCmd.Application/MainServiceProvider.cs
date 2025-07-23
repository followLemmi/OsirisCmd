using System;
using Microsoft.Extensions.DependencyInjection;
using OsirisCmd.Services;

namespace Application;

public static class MainServiceProvider
{
    public static IServiceProvider ServiceProvider { get; private set; } = null!;    
    
    public static void Build()
    {
        var services = new ServiceCollection();

        services.AddOsirisCmdServices();
        
        ServiceProvider = services.BuildServiceProvider();
    }
}