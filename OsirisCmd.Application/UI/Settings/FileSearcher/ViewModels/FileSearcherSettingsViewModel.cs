using Application.Core.Services.SettingsManager;
using Application.Services;
using Application.Services.FileSearcher.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Application.UI.Settings.FileSearcher.ViewModels;

public class FileSearcherSettingsViewModel
{
    
    public FileSearcherSettings? Settings { get; set; }

    public FileSearcherSettingsViewModel()
    {
        Settings = MainServiceProvider.ServiceProvider.GetRequiredService<ISettingsProviderService>().AttachSettings<FileSearcherSettings>();
    }
    
}