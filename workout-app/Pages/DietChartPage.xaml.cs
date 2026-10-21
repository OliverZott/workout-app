namespace workout_app.Pages;

public partial class DietChartPage : ContentPage
{
    public DietChartPage(DietChartPageModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is DietChartPageModel vm)
            await vm.LoadAsync();
    }
}
