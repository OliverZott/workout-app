namespace workout_app;

public partial class App : Application
{
    public App(DatabaseService database)
    {
        InitializeComponent();

        // Run DB seed on startup
        Task.Run(async () =>
        {
            await database.SeedWeightDataAsync();
        });


        // Force Light Mode - Prevent automatic dark mode switching
        UserAppTheme = AppTheme.Light;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}