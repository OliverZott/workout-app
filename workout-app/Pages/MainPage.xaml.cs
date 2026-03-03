using workout_app.PageModels;

namespace workout_app.Pages;

public partial class MainPage : ContentPage
{
    public MainPage(MainPageModel mainPageModel)
    {
        InitializeComponent();
        BindingContext = mainPageModel;
    }
}
