namespace OsirisCmd.UI;

public static class UIServiceProviderAdapter
{
    
    public static IServiceProvider ServiceProvider { get; private set; }
    
    public static void InjectMainServiceProvider(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }
    
}