using System.IO;

namespace OsirisCmdPluginsManager;

public class PluginManifest
{
    public string MainDllName { get; set; }

    public static PluginManifest ParsePluginManifest(string manifestFilePath)
    {
        if (Path.GetFileNameWithoutExtension(manifestFilePath) != "manifest" || Path.GetExtension(manifestFilePath) != ".plug")
        {
            throw new FileLoadException("Manifest file has invalid name or extension.");
        }
        var fileLines = File.ReadAllLines(manifestFilePath);
        if (fileLines.Length == 0)
        {
            throw new FileLoadException("Manifest file is empty.");
        }
        var manifest = new PluginManifest();
        foreach (var line in fileLines)
        {
            var parts = line.Split('=');
            if (parts.Length != 2)
            {
                throw new FileLoadException("Manifest file has invalid format.");
            }
            var key = parts[0].Trim();
            var value = parts[1].Trim();

            switch (key)
            {
                case "main_dll_name":
                    manifest.MainDllName = value;
                    break;
                default:
                    throw new FileLoadException($"Unknown key '{key}' in manifest file.");
            }
        }
        return manifest;
    }
}