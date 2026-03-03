using CommunityToolkit.Mvvm.ComponentModel;

namespace workout_app.PageModels.Controls;

public partial class DateTimePickerModel : ObservableObject
{
    [ObservableProperty]
    private DateTime selectedDate = DateTime.Now;
    [ObservableProperty]
    private TimeSpan selectedTime = DateTime.Now.TimeOfDay;
    [ObservableProperty]
    private string selectedDateDisplay = string.Empty;
    [ObservableProperty]
    private string selectedTimeDisplay = string.Empty;

    public DateTimePickerModel()
    {
        UpdateDisplayValues();
    }

    partial void OnSelectedDateChanged(DateTime value)
    {
        UpdateDisplayValues();
    }

    partial void OnSelectedTimeChanged(TimeSpan value)
    {
        UpdateDisplayValues();
    }

    private void UpdateDisplayValues()
    {
        SelectedDateDisplay = SelectedDate.ToString("dd.MM.yyyy");
        SelectedTimeDisplay = SelectedTime.ToString(@"hh\:mm");
    }
}
