using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace workout_app.PageModels;

public partial class DietInsertPageModel : ObservableObject
{
    private readonly DatabaseService databaseService;

    public DateTimePickerModel DateTimePicker { get; } = new();

    public IReadOnlyList<MealType> AvailableMealTypes { get; } = [.. Enum.GetValues<MealType>()];

    // Localized names for the picker; SelectedMealIndex maps back into AvailableMealTypes
    public IReadOnlyList<string> AvailableMeals { get; }

    [ObservableProperty] private int selectedMealIndex = -1;

    [ObservableProperty] private string description = "";

    public IReadOnlyList<DietRatingOption> RatingOptions { get; } =
        [.. Enumerable.Range(DietData.MinRating, DietData.MaxRating).Select(r => new DietRatingOption(r))];

    public DietInsertPageModel(DatabaseService databaseService)
    {
        this.databaseService = databaseService;
        AvailableMeals = [.. AvailableMealTypes.Select(DietData.GetMealName)];
        SelectedMealIndex = AvailableMealTypes.ToList().IndexOf(GuessMealType(DateTime.Now));
    }

    [RelayCommand]
    private void SelectRating(DietRatingOption option)
    {
        foreach (var o in RatingOptions)
            o.IsSelected = o == option;
    }

    [RelayCommand]
    private async Task SaveDiet()
    {
        var rating = RatingOptions.FirstOrDefault(o => o.IsSelected)?.Value;
        if (SelectedMealIndex < 0 || rating == null) return;

        var meal = AvailableMealTypes[SelectedMealIndex];

        var alertMessage = $"{DietData.GetMealName(meal)}\n" +
                           $"{AppResources.diet_rating}: {rating}\n" +
                           $"{Description}";

        var confirm = await Shell.Current.DisplayAlertAsync(
            AppResources.displayalert_want_to_save, alertMessage,
            AppResources.button_yes, AppResources.button_no);

        if (!confirm) return;

        await databaseService.AddDietAsync(new DietData
        {
            Meal = meal,
            Rating = rating.Value,
            Description = Description?.Trim() ?? "",
            Timestamp = DateTimePicker.SelectedDate.Date + DateTimePicker.SelectedTime
        });

        await Shell.Current.GoToAsync("..");
    }

    // Preselects the most likely meal for the current time of day
    private static MealType GuessMealType(DateTime now) => now.Hour switch
    {
        < 10 => MealType.Breakfast,
        >= 11 and < 14 => MealType.Lunch,
        >= 17 and < 22 => MealType.Dinner,
        _ => MealType.Snack
    };
}

public partial class DietRatingOption(int value) : ObservableObject
{
    public int Value { get; } = value;

    [ObservableProperty] private bool isSelected;
}
