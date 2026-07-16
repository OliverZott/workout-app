namespace workout_app;

public partial class App : Application
{
    private readonly DatabaseService _database;

    public App(DatabaseService database)
    {
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
}