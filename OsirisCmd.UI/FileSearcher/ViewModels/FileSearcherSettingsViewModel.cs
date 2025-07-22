using OsirisCmd.Services.FileSearcher.Settings;

namespace OsirisCmd.UI.FileSearcher.ViewModels;

public class FileSearcherSettingsViewModel
{
    
    public FileSearcherSettings? Settings { get; set; }

    public FileSearcherSettingsViewModel()
    {
        //TODO insert new service here after refactoring
        // Settings = ServiceLocator.GetService<ISettingsProviderService>().AttachSettings<FileSearcherSettings>();
    }
    
}