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
    public int selectedHours;

    [ObservableProperty]
    private int selectedMinutes;

    [ObservableProperty]
    private double? distance;

    [ObservableProperty]
    private double? altitude;

    [ObservableProperty]
    private string description = "";


    public bool DistanceEnabled => SelectedActivityType != ActivityType.WeightLifting;

    public IReadOnlyList<ActivityType> AvailableActivities { get; } =
        [.. Enum.GetValues<ActivityType>()];

    public IReadOnlyList<int> HourOptions { get; } =
        [.. Enumerable.Range(0, 24)];

    public IReadOnlyList<int> MinuteOptions { get; } =
        [.. Enumerable.Range(0, 60)];

    public ActivityInsertPageModel(DatabaseService databaseService)
    {
        this.databaseService = databaseService;
    }

    [RelayCommand]
    private async Task SaveActivity()
    {
        if (SelectedActivityType == null) return;

        var duration = SelectedHours * 60 + SelectedMinutes;
        if (duration == 0) return;

        var alertMessage = $"{SelectedActivityType}\n" +
                           $"Dauer: {SelectedHours}h {SelectedMinutes}min\n" +
                           $"Distanz: {Distance} km\n" +
                           $"Höhenmeter: {Altitude ?? 0} m";

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
