using OsirisCmd.Core.Services.Logger;
using Serilog;

namespace OsirisCmd.Services.Services.Logger;

public class LoggerService : ILoggerService
{

    private readonly string FileLoggerTemplate = "{Timestamp:dd-MM-yyyy HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}";

    public LoggerService() {
        ConfigureLogger();
    } 

    private void ConfigureLogger()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "/OsirisCmd/appdata/logs/osriscmd.log", outputTemplate: FileLoggerTemplate)
            .WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Debug, outputTemplate: FileLoggerTemplate)
            .CreateLogger();
    }

    public void LogDebug(string message)
    {
        Log.Debug(message);
    }

    public void LogError(string message)
    {
        Log.Error(message);
    }

    public void LogFatal(string message, Exception ex)
    {
        Log.Fatal(ex, message);
    }
}
