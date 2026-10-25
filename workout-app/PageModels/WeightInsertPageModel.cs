using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace workout_app.PageModels;

public partial class WeightInsertPageModel : ObservableObject, IQueryAttributable
{
    private readonly DatabaseService databaseService;
    private readonly ILogger<WeightInsertPageModel> logger;

    // Set when the page was opened from a list entry; saving then updates this entry instead of adding a new one
    private WeightData? editEntry;

    public DateTimePickerModel DateTimePicker { get; } = new();

    [ObservableProperty]
    private double? lastSelectedWeight;

    [ObservableProperty]
    public double? selectedWeight;

    public WeightInsertPageModel(DatabaseService databaseService, ILogger<WeightInsertPageModel> logger)
    {
        this.databaseService = databaseService;
        this.logger = logger;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!query.TryGetValue("Entry", out var value) || value is not WeightData entry) return;

        editEntry = entry;
        SelectedWeight = entry.Weight;
        DateTimePicker.SelectedDate = entry.Timestamp.Date;
        DateTimePicker.SelectedTime = entry.Timestamp.TimeOfDay;
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

        var entry = new WeightData
        {
            Id = editEntry?.Id ?? 0,
            Weight = SelectedWeight ?? 0,
            Timestamp = DateTimePicker.SelectedDate.Add(DateTimePicker.SelectedTime)
        };

        try
        {
            if (editEntry != null)
                await databaseService.UpdateAsync(entry);
            else
                await databaseService.AddWeightAsync(entry);
        }
        catch (Exception ex)
        {
            // Stay on the page so the input is not lost
            logger.LogError(ex, "Saving weight entry failed");
            await Shell.Current.DisplayAlertAsync(AppResources.displayalert_save_failed, AppResources.displayalert_error_message, AppResources.button_ok);
            return;
        }

        await Shell.Current.GoToAsync("..");
    }
}
