using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace workout_app.PageModels;

public partial class WeightChartPageModel : ObservableObject
{
    private bool IsLoading;
    private readonly DatabaseService databaseService;
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

    public WeightChartPageModel(DatabaseService databaseService)
    {
        _selectedRange = RangeType.Week;
        this.databaseService = databaseService;
    }

    public Task LoadAsync() => LoadDataAsync();

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

            var (from, to) = GetDateRange();

            var filteredData = await databaseService.GetWeightsAsync(from, to);

            // Update on main thread
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Data = new ObservableCollection<WeightData>(filteredData);
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
