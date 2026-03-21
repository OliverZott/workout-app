namespace workout_app.Pages;

public partial class WeightInsertPage : ContentPage
{
    public WeightInsertPage(WeightInsertPageModel weightInsertPageModel)
    {
        InitializeComponent();
        BindingContext = weightInsertPageModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is WeightInsertPageModel vm)
            await vm.OnAppearingAsync();
    }
}