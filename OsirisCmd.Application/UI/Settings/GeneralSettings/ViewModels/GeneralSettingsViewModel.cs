using Application.Core.Services.SettingsManager;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application.UI.Settings.GeneralSettings.ViewModels;

public class GeneralSettingsViewModel
{
    public GeneralSettings? Settings { get; set; }

    public GeneralSettingsViewModel()
    {
        Settings = MainServiceProvider.ServiceProvider.GetRequiredService<ISettingsProviderService>().AttachSettings<global::Application.UI.Settings.GeneralSettings.GeneralSettings>();
    }
    
}