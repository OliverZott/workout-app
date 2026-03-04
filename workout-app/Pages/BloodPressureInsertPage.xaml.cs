namespace workout_app.Pages;

public partial class BloodPressureInsertPage : ContentPage
{
    public BloodPressureInsertPage(BloodPressureInsertPageModel pageModel)
    {
        InitializeComponent();
        BindingContext = pageModel;
    }
}