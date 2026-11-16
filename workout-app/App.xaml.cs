using System.Globalization;

namespace workout_app;

public partial class App : Application
{
    private readonly DatabaseService _database;
    private readonly WorkoutReminderService _workoutReminder;
    private readonly MedicationReminderService _medicationReminders;
    private readonly ILogger<App> _logger;

    public App(DatabaseService database, WorkoutReminderService workoutReminder,
        MedicationReminderService medicationReminders, ILogger<App> logger)
    {
        var englishCulture = CultureInfo.GetCultureInfo("en");
        CultureInfo.DefaultThreadCurrentUICulture = englishCulture;
        AppResources.Culture = englishCulture;

        InitializeComponent();
        _database = database;
        _workoutReminder = workoutReminder;
        _medicationReminders = medicationReminders;
        _logger = logger;

        UserAppTheme = Preferences.Get("IsDarkThemeEnabled", false) ? AppTheme.Dark : AppTheme.Light;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var shell = new AppShell();
        // Breadcrumbs in the log file: shows which page the user was on before an error
        shell.Navigated += (_, e) => _logger.LogInformation("Navigated to {Location}", e.Current.Location);
        return new Window(shell);
    }

    protected override async void OnStart()
    {
        base.OnStart();
        _logger.LogInformation("App started, version {Version}", AppInfo.Current.VersionString);
        await InitializeDatabaseAsync();

        // Alarms are wiped by force-stop/redeploy, so re-create them on every start.
        // After the DB init, because the medication reminders are built from the plan in the DB.
        // Both catch and log their own errors.
        await _workoutReminder.RescheduleAsync();
        await _medicationReminders.RescheduleAsync();
    }

    // Also runs when returning from the "All files access" screen after a fresh install.
    protected override async void OnResume()
    {
        base.OnResume();
        await InitializeDatabaseAsync();
    }

    // async void callers: an exception escaping here would crash the app
    private async Task InitializeDatabaseAsync()
    {
        try
        {
            await _database.InitializeAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database initialization failed");
        }
    }
}
