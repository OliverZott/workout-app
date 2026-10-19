using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace workout_app.PageModels;

public partial class BloodPressureChartPageModel : ObservableObject
{
    private const int PageSize = 20;
    private readonly DatabaseService databaseService;
    private RangeType _selectedRange;
    private List<BloodPressureData> allMeasurements = [];
    private int nextPageStartIndex;

    [ObservableProperty]
    private ObservableCollection<BloodPressureData> data = [];

    [ObservableProperty]
    private ObservableCollection<BloodPressureData> visibleMeasurements = [];

    [ObservableProperty]
    private bool isLoadingMore;

    public bool HasMoreMeasurements => nextPageStartIndex < allMeasurements.Count;

    [ObservableProperty]
    private bool isLoading = false;

    [ObservableProperty]
    private string lastBloodPressureReading = "--/--";

    [ObservableProperty]
    private string averageBloodPressureReading = "--/--";

    [ObservableProperty]
    private double minBloodPressureWithPadding;

    [ObservableProperty]
    private double maxBloodPressureWithPadding;


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
                allMeasurements = filteredData.OrderByDescending(x => x.Timestamp).ToList();
                ResetVisibleMeasurements();
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading data: {ex.Message}");

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Data = [];
                allMeasurements = [];
                ResetVisibleMeasurements();
            });
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void LoadMoreMeasurements()
    {
        if (IsLoadingMore || !HasMoreMeasurements)
            return;

        IsLoadingMore = true;

        foreach (var item in allMeasurements.Skip(nextPageStartIndex).Take(PageSize))
            VisibleMeasurements.Add(item);

        nextPageStartIndex += PageSize;
        OnPropertyChanged(nameof(HasMoreMeasurements));

        IsLoadingMore = false;
    }

    private void ResetVisibleMeasurements()
    {
        // Assign the first page in one go: replacing the collection and then adding items one by one
        // makes the iOS CollectionView lay out the items over the header (chart) while it reloads.
        VisibleMeasurements = new ObservableCollection<BloodPressureData>(allMeasurements.Take(PageSize));
        nextPageStartIndex = VisibleMeasurements.Count;
        OnPropertyChanged(nameof(HasMoreMeasurements));
    }

    private (DateTime from, DateTime to) GetDateRange()
    {
        var today = DateTime.Today;
        var from = SelectedRange switch
        {
            RangeType.Week  => today.AddDays(-6),
            RangeType.Month => today.AddMonths(-1).AddDays(1),
            RangeType.Max   => DateTime.MinValue,
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

            var min = Data.Min(d => Math.Min(d.Diastolic, d.HeartRate));
            var max = Data.Max(d => Math.Max(d.Systolic, d.HeartRate));
            MinBloodPressureWithPadding = min - 10;
            MaxBloodPressureWithPadding = max + 10;
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
