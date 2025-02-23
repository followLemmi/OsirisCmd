namespace OsirisCmd.core.PluginManager;

public class Plugin(string name, string description, string version)
{
    public string Name { get; } = name;
    public string Description { get; } = description;
    public string Version { get; } = version;
}