using Microsoft.Extensions.DependencyInjection;
using OsirisCmd.Core.Services.SettingsManager;
using OsirisCmd.Services.Services.FileSearcher.Settings;

namespace OsirisCmd.UI.Settings.FileSearcher.ViewModels;

public class FileSearcherSettingsViewModel
{
    
    public FileSearcherSettings? Settings { get; set; }

    public FileSearcherSettingsViewModel()
    {
        Settings = UIServiceProviderAdapter.ServiceProvider.GetRequiredService<ISettingsProviderService>().AttachSettings<FileSearcherSettings>();
    }
    
}