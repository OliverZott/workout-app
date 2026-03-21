namespace workout_app.Pages;

public partial class BloodPressureChartPage : ContentPage
{
    public BloodPressureChartPage(BloodPressureChartPageModel bloodPressureChartPageModel)
    {
        InitializeComponent();
        BindingContext = bloodPressureChartPageModel;
    }

    // Refresh because we navigate back via ".." and the page is not recreated, so we need to refresh the data when we come back to this page.
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is BloodPressureChartPageModel vm)
        {
            await vm.LoadAsync();
        }
    }
}