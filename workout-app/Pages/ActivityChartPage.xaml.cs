namespace workout_app.Pages;

public partial class ActivityChartPage : ContentPage
{
    public ActivityChartPage(ActivityChartPageModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}

