using Serilog;

namespace workout_app;

public static class SerilogConfig
{
    public static Serilog.ILogger ConfigureLogging()
    {
        var logFilePath = Path.Combine(FileSystem.AppDataDirectory, "log-.log");

        var config = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.Debug();

        if (Preferences.Get("IsLoggingEnabled", false))
        {
            config.WriteTo.File(
                logFilePath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 3,
                flushToDiskInterval: TimeSpan.FromMilliseconds(100),
                fileSizeLimitBytes: 1_000_000,
                shared: true);
        }

        return Log.Logger = config.CreateLogger();
    }
}
