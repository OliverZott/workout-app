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
    }


    public Task LoadAsync() => LoadDataAsync();

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

            var (from, to) = GetDateRange();
            var filteredData = await databaseService.GetCardioAsync(from, to);

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

    private (DateTime from, DateTime to) GetDateRange()
    {
        var today = DateTime.Today;
        var from = SelectedRange switch
        {
            RangeType.Week  => today.AddDays(-6),
            RangeType.Month => today.AddMonths(-1).AddDays(1),
            RangeType.Max   => today.AddDays(-89),
            _               => today.AddDays(-6)
        };
        return (from, today.AddDays(1));
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
