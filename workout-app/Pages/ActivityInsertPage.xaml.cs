namespace workout_app.Pages;

public partial class ActivityInsertPage : ContentPage
{
    public ActivityInsertPage(ActivityInsertPageModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
