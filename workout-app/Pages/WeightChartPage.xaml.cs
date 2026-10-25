using workout_app.PageModels;

namespace workout_app.Pages;

public partial class WeightChartPage : ContentPage
{
    // Typed access for bindings inside the list items (via x:Reference), which can't use RelativeSource safely on recycled rows
    public WeightChartPageModel ViewModel { get; }

    public WeightChartPage(WeightChartPageModel weightChartPageModel)
    {
        ViewModel = weightChartPageModel;
        InitializeComponent();
        BindingContext = ViewModel;
    }

    // Refresh because we navigate back via ".." and the page is not recreated, so we need to refresh the data when we come back to this page.
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ViewModel.LoadAsync();
    }
}