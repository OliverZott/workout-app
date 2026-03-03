using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace workout_app.PageModels;

public partial class WeightChartPageModel : ObservableObject
{
    private bool IsLoading;
    private readonly MockDataService mockDataService;
    private RangeType _selectedRange;

    [ObservableProperty]
    private ObservableCollection<WeightData> data = [];

    [ObservableProperty]
    private string lastWeightReading = "--";

    [ObservableProperty]
    private string averageWeightReading = "--";

    [ObservableProperty]
    public double minWeightWithPadding;

    [ObservableProperty]
    public double maxWeightWithPadding;

    public WeightChartPageModel(MockDataService mockDataService)
    {
        _selectedRange = RangeType.Week;
        this.mockDataService = mockDataService;
        _ = LoadDataAsync();
    }

    public Task RefreshAsync() => LoadDataAsync();

    partial void OnDataChanged(ObservableCollection<WeightData> value)
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
            var apiData = await GetWeightDataFromApi(start, end);

            // Update on main thread
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Data = new ObservableCollection<WeightData>(apiData);
            });

            MinWeightWithPadding = (int)(Data?.Min(d => d.Weight) ?? 0) - 2;
            MaxWeightWithPadding = (int)(Data?.Max(d => d.Weight) ?? 0) + 2;
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
            RangeType.Max => end.AddDays(-90), // Show last year for "Max"
            _ => end.AddDays(-7)
        };
        return (start, end);
    }

    private async Task<IList<WeightData>> GetWeightDataFromApi(DateTime startDate, DateTime endDate)
    {
        await Task.Delay(500); // Simulate API delay
        var data = mockDataService.GetWeightData(startDate, endDate);
        return data;
    }

    private void UpdateCalculatedValues()
    {
        if (Data?.Any() == true)
        {
            var avgWeight = Math.Round(Data.Average(d => d.Weight), 1);
            AverageWeightReading = $"{avgWeight}";

            var latestWeight = Math.Round(Data.OrderByDescending(d => d.Timestamp).First().Weight, 1);
            LastWeightReading = $"{latestWeight}";
        }
    }


    [RelayCommand]
    public async Task GoToInsertWeight()
    {
        await Shell.Current.GoToAsync(nameof(Pages.WeightInsertPage));
    }
}
