namespace workout_app.Pages;

public partial class MotionSensorPage : ContentPage
{
    public MotionSensorPage(MotionSensorPageModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (BindingContext is MotionSensorPageModel vm)
            vm.StopTracking();
    }
}
