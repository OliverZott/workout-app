namespace workout_app.Pages;

public partial class ActivityChartPage : ContentPage
{
    // Typed access for bindings inside the list items (via x:Reference), which can't use RelativeSource safely on recycled rows
    public ActivityChartPageModel ViewModel { get; }

    public ActivityChartPage(ActivityChartPageModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();
        BindingContext = ViewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ViewModel.LoadAsync();
    }
}

