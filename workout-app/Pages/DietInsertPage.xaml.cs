namespace workout_app.Pages;

public partial class DietInsertPage : ContentPage
{
    public DietInsertPage(DietInsertPageModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
