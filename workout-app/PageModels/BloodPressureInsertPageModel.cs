using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace workout_app.PageModels;

public partial class BloodPressureInsertPageModel : ObservableObject
{
    public DateTimePickerModel DateTimePicker { get; } = new();

    private readonly DatabaseService databaseService;

    [ObservableProperty]
    private int selectedSystolic = 130;
    [ObservableProperty]
    private int selectedDiastolic = 75;
    [ObservableProperty]
    private int selectedPulse = 70;

    public ObservableCollection<int> BloodPressureValues => new(Enumerable.Range(50, 250));
    public ObservableCollection<int> DiastolicValues => new(Enumerable.Range(30, 180));
    public ObservableCollection<int> PulsValues => new(Enumerable.Range(30, 180));

    public BloodPressureInsertPageModel(DatabaseService databaseService)
    {
        this.databaseService = databaseService;
    }

    [RelayCommand]
    private async Task SaveBloodPressure()
    {
        var alertMessage = $"{AppResources.systolic}: {SelectedSystolic}\n{AppResources.diastolic}: {SelectedDiastolic}\n{AppResources.heartrate}: {SelectedPulse}";
        var saveBloodPressure = await Shell.Current.DisplayAlertAsync(AppResources.displayalert_want_to_save, alertMessage, AppResources.button_yes, AppResources.button_no);

        if (!saveBloodPressure) return;

        await databaseService.AddCardioAsync(new BloodPressureData
        {
            Systolic = SelectedSystolic,
            Diastolic = SelectedDiastolic,
            HeartRate = SelectedPulse,
            Timestamp = DateTimePicker.SelectedDate.Date + DateTimePicker.SelectedTime
        });

        await Shell.Current.GoToAsync("..");
    }
}
