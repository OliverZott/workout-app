namespace workout_app.Pages;

public partial class MedicationPage : ContentPage
{
    // Typed access for bindings inside the list items (via x:Reference)
    public MedicationPageModel ViewModel { get; }

    public MedicationPage(MedicationPageModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();
        BindingContext = ViewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ViewModel.LoadAsync();
    }

    // Persist on leave so edits are kept even if the user navigates back without pressing "Done"
    protected override async void OnDisappearing()
    {
        base.OnDisappearing();
        if (ViewModel.IsEditing)
        {
            ViewModel.IsEditing = false;
            await ViewModel.SaveAsync();
        }
    }
}
