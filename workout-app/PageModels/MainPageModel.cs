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
}
