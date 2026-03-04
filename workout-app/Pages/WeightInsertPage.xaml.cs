namespace workout_app.Pages;

public partial class WeightInsertPage : ContentPage
{
    public WeightInsertPage(WeightInsertPageModel weightInsertPageModel)
    {
        InitializeComponent();
        BindingContext = weightInsertPageModel;
    }
}