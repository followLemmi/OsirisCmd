using System.Security.Cryptography;
using Microsoft.Extensions.DependencyInjection;

namespace OsirisCmd.DI;

public class DIContainer
{
    public static void Initialize(Action<IServiceCollection> configureContainer)
    {
        var serviceCollection = new ServiceCollection();
        
        configureContainer(serviceCollection);
        
        ServiceLocator.Initialize(serviceCollection.BuildServiceProvider());
    }
    
}