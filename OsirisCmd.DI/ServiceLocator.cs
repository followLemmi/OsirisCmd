using Microsoft.Extensions.DependencyInjection;

namespace OsirisCmd.DI;

public static class ServiceLocator
{
    private static IServiceProvider? _serviceProvider;

    public static IServiceProvider ServiceProvider
    {
        get => _serviceProvider! ?? throw new Exception("Service provider is not initialized");
        set => _serviceProvider = value;
    }

    internal static void Initialize(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    public static T GetService<T>() where T : class
    {
        return ServiceProvider.GetRequiredService<T>();
    }
}