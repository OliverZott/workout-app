using workout_app.PageModels;

namespace workout_app.Pages;

public partial class WeightChartPage : ContentPage
{
    public WeightChartPage(WeightChartPageModel weightChartPageModel)
    {
        InitializeComponent();
        BindingContext = weightChartPageModel;
    }

    // Refresh because we navigate back via ".." and the page is not recreated, so we need to refresh the data when we come back to this page.
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is WeightChartPageModel vm)
        {
            await vm.RefreshAsync();
        }
    }
}