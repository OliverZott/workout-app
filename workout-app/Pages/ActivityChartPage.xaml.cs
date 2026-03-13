namespace workout_app.Pages;

public partial class ActivityChartPage : ContentPage
{
    public ActivityChartPage(ActivityChartPageModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ActivityChartPageModel vm)
            await vm.RefreshAsync();
    }
}

