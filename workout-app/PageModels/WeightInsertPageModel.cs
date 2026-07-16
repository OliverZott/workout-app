using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace workout_app.PageModels;

public partial class WeightInsertPageModel : ObservableObject
{
    private readonly DatabaseService databaseService;

    public DateTimePickerModel DateTimePicker { get; } = new();

    [ObservableProperty]
    private double? lastSelectedWeight;

    [ObservableProperty]
    public double? selectedWeight;

    public WeightInsertPageModel(DatabaseService databaseService)
    {
        this.databaseService = databaseService;
    }

    public async Task OnAppearingAsync()
    {
        var lastWeightEntry = await databaseService.GetLastWeightDataEntry();
        LastSelectedWeight = Math.Round(lastWeightEntry?.Weight ?? 70d, 1);
    }


    [RelayCommand]
    private async Task SaveWeight()
    {
        if (SelectedWeight == null) return;

        var alertMessage = $"{AppResources.weight}: {SelectedWeight}";
        var saveBloodPressure = await Shell.Current.DisplayAlertAsync(AppResources.displayalert_want_to_save, alertMessage, AppResources.button_yes, AppResources.button_no);

        if (!saveBloodPressure) return;

        await databaseService.AddWeightAsync(new WeightData
        {
            Weight = SelectedWeight ?? 0,
            Timestamp = DateTimePicker.SelectedDate.Add(DateTimePicker.SelectedTime)
        });

        await Shell.Current.GoToAsync("..");
    }
}
