namespace OsirisCmd.Core.Services.Logger;

public interface ILoggerService
{
    void LogDebug(string message);

    void LogError(string message);

    void LogFatal(string message, Exception e);

}
