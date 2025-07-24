namespace OsirisCmd.Core.Services.SettingsManager;

public interface ISettingsProviderService
{

    T? AttachSettings<T>() where T : class, ISettings, new();

}
