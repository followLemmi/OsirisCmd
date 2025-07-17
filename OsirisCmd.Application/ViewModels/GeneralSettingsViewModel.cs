using Application.Settings;
using OsirisCmd.DI;
using OsirisCmd.SettingsManager;

namespace Application.ViewModels;

public class GeneralSettingsViewModel
{
    public GeneralSettings Settings { get; set; }

    public GeneralSettingsViewModel()
    {
        Settings = ServiceLocator.GetService<ISettingsProviderService>().AttachSettings<GeneralSettings>();
    }
    
}