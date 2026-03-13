using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace workout_app.PageModels;

public partial class ActivityInsertPageModel : ObservableObject
{
    private readonly DatabaseService databaseService;

    public DateTimePickerModel DateTimePicker { get; } = new();

    [ObservableProperty]
    private ActivityType selectedActivityType = ActivityType.Hiking;

    [ObservableProperty]
    private double? distance;

    [ObservableProperty]
    private double? altitude;

    [ObservableProperty]
    private string description = "";

    public IReadOnlyList<ActivityType> AvailableActivities { get; } =
        Enum.GetValues<ActivityType>().ToList();

    public ActivityInsertPageModel(DatabaseService databaseService)
    {
        this.databaseService = databaseService;
    }

    [RelayCommand]
    private async Task SaveActivity()
    {
        if (Distance == null || Distance <= 0) return;

        var alertMessage = $"{SelectedActivityType}\n" +
                           $"Distanz: {Distance} km\n" +
                           $"Höhenmeter: {Altitude ?? 0} m";

        var confirm = await Shell.Current.DisplayAlertAsync(
            AppResources.displayalert_want_to_save, alertMessage,
            AppResources.button_yes, AppResources.button_no);

        if (!confirm) return;

        await databaseService.AddActivityAsync(new ActivityData
        {
            Type = SelectedActivityType,
            Distance = Distance ?? 0,
            Altitude = Altitude ?? 0,
            Description = Description ?? "",
            Timestamp = DateTimePicker.SelectedDate.Date + DateTimePicker.SelectedTime
        });

        await Shell.Current.GoToAsync("..");
    }
}
