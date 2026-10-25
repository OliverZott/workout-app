using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace workout_app.PageModels;

public partial class ActivityInsertPageModel : ObservableObject, IQueryAttributable
{
    private readonly DatabaseService databaseService;
    private readonly ILogger<ActivityInsertPageModel> logger;

    // Set when the page was opened from a list entry; saving then updates this entry instead of adding a new one
    private ActivityData? editEntry;

    public DateTimePickerModel DateTimePicker { get; } = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DistanceEnabled))]
    private ActivityType? selectedActivityType = null;

    [ObservableProperty] private int? selectedHours;

    [ObservableProperty] private int? selectedMinutes;

    [ObservableProperty] private double? distance;

    [ObservableProperty] private double? altitude;

    [ObservableProperty] private string description = "";

    public bool DistanceEnabled => SelectedActivityType != ActivityType.WeightLifting;

    public IReadOnlyList<ActivityType> AvailableActivities { get; } = [.. Enum.GetValues<ActivityType>()];

    public ActivityInsertPageModel(DatabaseService databaseService, ILogger<ActivityInsertPageModel> logger)
    {
        this.databaseService = databaseService;
        this.logger = logger;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (!query.TryGetValue("Entry", out var value) || value is not ActivityData entry) return;

        editEntry = entry;
        SelectedActivityType = entry.Type;
        SelectedHours = entry.Duration / 60;
        SelectedMinutes = entry.Duration % 60;
        Distance = entry.Distance > 0 ? entry.Distance : null;
        Altitude = entry.Altitude > 0 ? entry.Altitude : null;
        Description = entry.Description;
        DateTimePicker.SelectedDate = entry.Timestamp.Date;
        DateTimePicker.SelectedTime = entry.Timestamp.TimeOfDay;
    }

    [RelayCommand]
    private async Task SaveActivity()
    {
        if (SelectedActivityType == null) return;

        var hours = Math.Clamp(Convert.ToInt32(SelectedHours), 0, 23);
        var minutes = Math.Clamp(Convert.ToInt32(SelectedMinutes), 0, 59);
        var duration = hours * 60 + minutes;
        if (duration == 0) return;

        var alertMessage = $"{SelectedActivityType}\n" +
                           $"{AppResources.activity_alert_duration_label}: {hours}{AppResources.unit_h} {minutes}{AppResources.unit_min}\n" +
                           $"{AppResources.activity_alert_distance_label}: {Distance} {AppResources.unit_km}\n" +
                           $"{AppResources.activity_alert_altitude_label}: {Altitude ?? 0} m";

        var confirm = await Shell.Current.DisplayAlertAsync(
            AppResources.displayalert_want_to_save, alertMessage,
            AppResources.button_yes, AppResources.button_no);

        if (!confirm) return;


        var entry = new ActivityData
        {
            Id = editEntry?.Id ?? 0,
            Type = SelectedActivityType.Value,
            Duration = duration,
            Distance = Distance ?? 0,
            Altitude = Altitude ?? 0,
            Description = Description ?? "",
            Timestamp = DateTimePicker.SelectedDate.Date + DateTimePicker.SelectedTime
        };

        try
        {
            if (editEntry != null)
                await databaseService.UpdateAsync(entry);
            else
                await databaseService.AddActivityAsync(entry);
        }
        catch (Exception ex)
        {
            // Stay on the page so the input is not lost
            logger.LogError(ex, "Saving activity entry failed");
            await Shell.Current.DisplayAlertAsync(AppResources.displayalert_save_failed, AppResources.displayalert_error_message, AppResources.button_ok);
            return;
        }

        await Shell.Current.GoToAsync("..");
    }
}
