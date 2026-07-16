using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace workout_app.PageModels;

public partial class ActivityInsertPageModel : ObservableObject
{
    private readonly DatabaseService databaseService;

    public DateTimePickerModel DateTimePicker { get; } = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DistanceEnabled))]
    private ActivityType? selectedActivityType = null;

    [ObservableProperty]
    private int? selectedHours;

    [ObservableProperty]
    private int? selectedMinutes;

    [ObservableProperty]
    private double? distance;

    [ObservableProperty]
    private double? altitude;

    [ObservableProperty]
    private string description = "";


    public bool DistanceEnabled => SelectedActivityType != ActivityType.WeightLifting;

    public IReadOnlyList<ActivityType> AvailableActivities { get; } =
        [.. Enum.GetValues<ActivityType>()];

    public ActivityInsertPageModel(DatabaseService databaseService)
    {
        this.databaseService = databaseService;
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


        await databaseService.AddActivityAsync(new ActivityData
        {
            Type = SelectedActivityType.Value,
            Duration = duration,
            Distance = Distance ?? 0,
            Altitude = Altitude ?? 0,
            Description = Description ?? "",
            Timestamp = DateTimePicker.SelectedDate.Date + DateTimePicker.SelectedTime
        });

        await Shell.Current.GoToAsync("..");
    }
}
