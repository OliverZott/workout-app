namespace workout_app;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Force Light Mode - Prevent automatic dark mode switching
        UserAppTheme = AppTheme.Light;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}