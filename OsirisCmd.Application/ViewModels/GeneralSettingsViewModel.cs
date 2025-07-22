using Application.Settings;
using OsirisCmd.Core.SettingsManager;
using OsirisCmd.DI;

namespace Application.ViewModels;

public class GeneralSettingsViewModel
{
    public GeneralSettings Settings { get; set; }

    public GeneralSettingsViewModel()
    {
        Settings = ServiceLocator.GetService<ISettingsProviderService>().AttachSettings<GeneralSettings>();
    }
    
}