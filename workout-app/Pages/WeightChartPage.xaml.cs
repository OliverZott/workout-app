using workout_app.PageModels;

namespace workout_app.Pages;

public partial class WeightChartPage : ContentPage
{
    public WeightChartPage(WeightChartPageModel weightChartPageModel)
    {
        InitializeComponent();
        BindingContext = weightChartPageModel;
    }
}