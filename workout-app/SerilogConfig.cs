using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace workout_app;

public static class SerilogConfig
{
    public const string LogFilePattern = "log-*.log";
    public static string LogDirectory => FileSystem.AppDataDirectory;

    private const string OutputTemplate =
        "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}";

    // Warnings and errors always go to the file; the "Enable Logging" setting adds Information (e.g. navigation).
    private static readonly LoggingLevelSwitch FileLevel = new(LogEventLevel.Warning);

    public static Serilog.ILogger CreateLogger()
    {
        SetDetailedFileLogging(Preferences.Get("IsLoggingEnabled", false));

        return Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .WriteTo.Debug(outputTemplate: OutputTemplate)
            // Rotates daily and at 1 MB, keeps at most 5 files (~5 MB in total)
            .WriteTo.File(
                Path.Combine(LogDirectory, "log-.log"),
                outputTemplate: OutputTemplate,
                levelSwitch: FileLevel,
                rollingInterval: RollingInterval.Day,
                rollOnFileSizeLimit: true,
                fileSizeLimitBytes: 1_000_000,
                retainedFileCountLimit: 5,
                flushToDiskInterval: TimeSpan.FromSeconds(1))
            .CreateLogger();
    }

    public static void SetDetailedFileLogging(bool enabled) =>
        FileLevel.MinimumLevel = enabled ? LogEventLevel.Information : LogEventLevel.Warning;
}
