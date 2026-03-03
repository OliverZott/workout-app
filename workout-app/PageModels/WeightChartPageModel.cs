using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace workout_app.PageModels;

public partial class WeightChartPageModel : ObservableObject
{
    [RelayCommand]
    public async Task GoToInsertWeight()
    {
        await Shell.Current.GoToAsync(nameof(Pages.WeightInsertPage));
    }
}
