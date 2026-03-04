using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace workout_app.PageModels;

public partial class MainPageModel : ObservableObject
{
    [RelayCommand]
    public async Task NavigateToWeight()
    {
        await Shell.Current.GoToAsync(nameof(Pages.WeightChartPage));
    }

    [RelayCommand]
    public async Task NavigateToCardio()
    {
        await Shell.Current.GoToAsync(nameof(Pages.BloodPressureChartPage));
    }

    [RelayCommand]
    public async Task NavigateToActivity()
    {
        await Shell.Current.GoToAsync(nameof(Pages.ActivityChartPage));
    }
}
