using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace workout_app.PageModels;

public partial class BloodPressureChartPageModel : ObservableObject
{
    private readonly DatabaseService databaseService;
    private RangeType _selectedRange;

    [ObservableProperty]
    private ObservableCollection<BloodPressureData> data = [];

    [ObservableProperty]
    private bool isLoading = false;

    [ObservableProperty]
    private string lastBloodPressureReading = "--/--";

    [ObservableProperty]
    private string averageBloodPressureReading = "--/--";



    public BloodPressureChartPageModel(DatabaseService databaseService)
    {
        _selectedRange = RangeType.Week;
        this.databaseService = databaseService;
        _ = LoadDataAsync();
    }


    public Task RefreshAsync() => LoadDataAsync();

    partial void OnDataChanged(ObservableCollection<BloodPressureData> value)
    {
        UpdateCalculatedValues();
    }

    public RangeType SelectedRange
    {
        get => _selectedRange;
        set
        {
            if (SetProperty(ref _selectedRange, value))
            {
                // Load new data when range changes
                _ = LoadDataAsync();
            }
        }
    }

    public ICommand SelectRangeCommand => new Command<string>(async (range) =>
    {
        if (Enum.TryParse(range, out RangeType selected))
        {
            SelectedRange = selected;
        }
    });

    private async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;

            var (start, end) = GetDateRange();
            var data = await databaseService.GetCardioAsync();
            var filteredData = data.Where(d => d.Timestamp.Date > start.Date && d.Timestamp.Date <= end.Date).ToList();

            // Update on main thread
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Data = new ObservableCollection<BloodPressureData>(filteredData);
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading data: {ex.Message}");

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Data = [];
            });
        }
        finally
        {
            IsLoading = false;
        }
    }

    private (DateTime start, DateTime end) GetDateRange()
    {
        var end = DateTime.Today;
        var start = SelectedRange switch
        {
            RangeType.Week => end.AddDays(-7),
            RangeType.Month => end.AddMonths(-1),
            RangeType.Max => end.AddDays(-90),
            _ => end.AddDays(-7)
        };
        return (start, end);
    }

    private void UpdateCalculatedValues()
    {
        if (Data?.Any() == true)
        {
            var avgSystolic = (int)Math.Round(Data.Average(d => d.Systolic));
            var avgDiastolic = (int)Math.Round(Data.Average(d => d.Diastolic));
            AverageBloodPressureReading = $"{avgSystolic}/{avgDiastolic}";

            var latestEntry = Data.OrderByDescending(d => d.Timestamp).First();
            LastBloodPressureReading = $"{latestEntry.Systolic}/{latestEntry.Diastolic}";
        }
        else
        {
            AverageBloodPressureReading = "--/--";
            LastBloodPressureReading = "--/--";
        }
    }


    [RelayCommand]
    public async Task GoToInsertCardio()
    {
        await Shell.Current.GoToAsync(nameof(Pages.BloodPressureInsertPage));
    }
}
