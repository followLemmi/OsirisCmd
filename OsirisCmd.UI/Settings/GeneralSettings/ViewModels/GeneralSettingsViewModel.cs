using Microsoft.Extensions.DependencyInjection;
using OsirisCmd.Core.Services.SettingsManager;

namespace OsirisCmd.UI.Settings.GeneralSettings.ViewModels;

public class GeneralSettingsViewModel
{
    public global::Application.Settings.GeneralSettings? Settings { get; set; }

    public GeneralSettingsViewModel()
    {
        Settings = UIServiceProviderAdapter.ServiceProvider.GetRequiredService<ISettingsProviderService>().AttachSettings<global::Application.Settings.GeneralSettings>();
    }
    
}