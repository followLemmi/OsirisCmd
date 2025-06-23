using System.Globalization;
using System.Resources;

namespace OsirisCmd.Localization;

public class LocalizationService
{
    
    private static ResourceManager _resourceManager;
    private static CultureInfo _cultureInfo = CultureInfo.CurrentUICulture;

    static LocalizationService()
    {
        _resourceManager = new ResourceManager("OsirisCmd.Localization.Localization", typeof(LocalizationService).Assembly);
    }
    
    public static string? GetString(string key)
    {
        return _resourceManager.GetString(key, _cultureInfo);
    }
    
    public static CultureInfo GetCurrentCulture()
    {
        return _cultureInfo;
    }
    public static void SetCurrentCulture(CultureInfo cultureInfo)
    {
        _cultureInfo = cultureInfo;
        CultureInfo.CurrentUICulture = cultureInfo;
    }
}