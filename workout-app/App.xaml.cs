using System.Globalization;

namespace workout_app;

public partial class App : Application
{
    private readonly DatabaseService _database;

    public App(DatabaseService database)
    {
        var englishCulture = CultureInfo.GetCultureInfo("en");
        CultureInfo.DefaultThreadCurrentUICulture = englishCulture;
        AppResources.Culture = englishCulture;

        InitializeComponent();
        _database = database;

        // Force Light Mode - Prevent automatic dark mode switching
        // TODO: implement dark mode support
        UserAppTheme = AppTheme.Light;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }

    protected override async void OnStart()
    {
        base.OnStart();
        await _database.InitializeAsync();
    }

    // Also runs when returning from the "All files access" screen after a fresh install.
    protected override async void OnResume()
    {
        base.OnResume();
        await _database.InitializeAsync();
    }
}