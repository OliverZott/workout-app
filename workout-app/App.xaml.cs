using System.Globalization;

namespace workout_app;

public partial class App : Application
{
    private readonly DatabaseService _database;
    private readonly ILogger<App> _logger;

    public App(DatabaseService database, ILogger<App> logger)
    {
        var englishCulture = CultureInfo.GetCultureInfo("en");
        CultureInfo.DefaultThreadCurrentUICulture = englishCulture;
        AppResources.Culture = englishCulture;

        InitializeComponent();
        _database = database;
        _logger = logger;

        // Force Light Mode - Prevent automatic dark mode switching
        // TODO: implement dark mode support
        UserAppTheme = AppTheme.Light;
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
