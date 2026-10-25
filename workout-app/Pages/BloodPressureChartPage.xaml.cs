namespace workout_app.Pages;

public partial class BloodPressureChartPage : ContentPage
{
    // Typed access for bindings inside the list items (via x:Reference), which can't use RelativeSource safely on recycled rows
    public BloodPressureChartPageModel ViewModel { get; }

    public BloodPressureChartPage(BloodPressureChartPageModel bloodPressureChartPageModel)
    {
        ViewModel = bloodPressureChartPageModel;
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