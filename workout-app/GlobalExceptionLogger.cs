using Serilog;

namespace workout_app;

// Writes unhandled exceptions to the log before the app terminates. The app still crashes as usual:
// continuing after an unknown error could leave the UI or the database in a broken state.
public static class GlobalExceptionLogger
{
    public static void Register()
    {
        AppDomain.CurrentDomain.UnhandledException += (_, e) => LogCrash(e.ExceptionObject as Exception);
        TaskScheduler.UnobservedTaskException += (_, e) =>
            Log.ForContext(typeof(GlobalExceptionLogger)).Error(e.Exception, "Unobserved task exception");

#if ANDROID
        // On Android unhandled exceptions arrive here instead of AppDomain.UnhandledException
        Android.Runtime.AndroidEnvironment.UnhandledExceptionRaiser += (_, e) => LogCrash(e.Exception);
#elif IOS
        // Required so exceptions from native callbacks reach AppDomain.UnhandledException (xamarin-macios#15252)
        ObjCRuntime.Runtime.MarshalManagedException += (_, e) =>
            e.ExceptionMode = ObjCRuntime.MarshalManagedExceptionMode.UnwindNativeCode;
#endif
    }

    // Flushes the file sink, otherwise the last entries are lost when the process dies
    private static void LogCrash(Exception? ex)
    {
        Log.ForContext(typeof(GlobalExceptionLogger)).Fatal(ex, "Unhandled exception, app is terminating");
        Log.CloseAndFlush();
    }
}
