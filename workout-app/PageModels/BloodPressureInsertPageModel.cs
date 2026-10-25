using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace workout_app.PageModels;

public partial class BloodPressureInsertPageModel : ObservableObject, IQueryAttributable
{
    public DateTimePickerModel DateTimePicker { get; } = new();

    private readonly DatabaseService databaseService;
    private readonly ILogger<BloodPressureInsertPageModel> logger;

    // Set when the page was opened from a list entry; saving then updates this entry instead of adding a new one
    private BloodPressureData? editEntry;

    [ObservableProperty]
    private int selectedSystolic = 130;
    [ObservableProperty]
    private int selectedDiastolic = 75;
    [ObservableProperty]
    private int selectedPulse = 70;

    public IReadOnlyList<int> BloodPressureValues { get; } = Enumerable.Range(50, 250).ToList();
    public IReadOnlyList<int> DiastolicValues { get; } = Enumerable.Range(30, 180).ToList();
    public IReadOnlyList<int> PulsValues { get; } = Enumerable.Range(30, 180).ToList();

    public BloodPressureInsertPageModel(DatabaseService databaseService, ILogger<BloodPressureInsertPageModel> logger)
    {
        this.databaseService = databaseService;
        this.logger = logger;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!query.TryGetValue("Entry", out var value) || value is not BloodPressureData entry) return;

        editEntry = entry;
        SelectedSystolic = entry.Systolic;
        SelectedDiastolic = entry.Diastolic;
        SelectedPulse = entry.HeartRate;
        DateTimePicker.SelectedDate = entry.Timestamp.Date;
        DateTimePicker.SelectedTime = entry.Timestamp.TimeOfDay;
    }

    [RelayCommand]
    private async Task SaveBloodPressure()
    {
        var alertMessage = $"{AppResources.systolic}: {SelectedSystolic}\n{AppResources.diastolic}: {SelectedDiastolic}\n{AppResources.heartrate}: {SelectedPulse}";
        var saveBloodPressure = await Shell.Current.DisplayAlertAsync(AppResources.displayalert_want_to_save, alertMessage, AppResources.button_yes, AppResources.button_no);

        if (!saveBloodPressure) return;

        var entry = new BloodPressureData
        {
            Id = editEntry?.Id ?? 0,
            Systolic = SelectedSystolic,
            Diastolic = SelectedDiastolic,
            HeartRate = SelectedPulse,
            Timestamp = DateTimePicker.SelectedDate.Date + DateTimePicker.SelectedTime
        };

        try
        {
            if (editEntry != null)
                await databaseService.UpdateAsync(entry);
            else
                await databaseService.AddCardioAsync(entry);
        }
        catch (Exception ex)
        {
            // Stay on the page so the input is not lost
            logger.LogError(ex, "Saving blood pressure entry failed");
            await Shell.Current.DisplayAlertAsync(AppResources.displayalert_save_failed, AppResources.displayalert_error_message, AppResources.button_ok);
            return;
        }

        await Shell.Current.GoToAsync("..");
    }
}
