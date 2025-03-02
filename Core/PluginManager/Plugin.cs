namespace OsirisCmd.core.PluginManager;

public class Plugin(string name, string description, string version, bool isEnabled)
{
    public string Name { get; } = name;
    public string Description { get; } = description;
    public string Version { get; } = version;
    public bool IsEnabled { get; } = isEnabled;
    
}