using workout_app.PageModels;

namespace workout_app.Pages;

public partial class SettingsPage : ContentPage
{
    public SettingsPage(SettingsPageModel settingsPageModel)
    {
        InitializeComponent();
        BindingContext = settingsPageModel;
    }
}
