using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace workout_app.PageModels;

public partial class WeightInsertPageModel : ObservableObject
{
    private readonly MockDataService mockDataService;

    public DateTimePickerModel DateTimePicker { get; } = new();

    [ObservableProperty]
    private double? lastSelectedWeight;

    [ObservableProperty]
    public double? selectedWeight;

    public WeightInsertPageModel(MockDataService mockDataService)
    {
        this.mockDataService = mockDataService;

        var lastWeightEntry = mockDataService.GetLastWeightDataEntry();
        LastSelectedWeight = Math.Round(lastWeightEntry?.Weight ?? 70d, 1);
    }

    [RelayCommand]
    private async Task SaveWeight()
    {
        if (SelectedWeight == null)
            return;

        var weight = new WeightData
        {
            Weight = SelectedWeight ?? 0,
            Timestamp = DateTimePicker.SelectedDate.Add(DateTimePicker.SelectedTime)
        };

        mockDataService.AddWeightDataEntry(weight);

        // Navigate back to the previous page (WeightChartPage) instead of
        // pushing a new instance of WeightChartPage onto the navigation stack.
        await Shell.Current.GoToAsync("..");
    }
}
