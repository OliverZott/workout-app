using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace workout_app.PageModels;

public partial class WeightChartPageModel : ObservableObject
{
    private const int PageSize = 20;
    private bool IsLoading;
    private readonly DatabaseService databaseService;
    private readonly ILogger<WeightChartPageModel> logger;
    private RangeType _selectedRange;
    private List<WeightData> allMeasurements = [];
    private int nextPageStartIndex;

    [ObservableProperty]
    private ObservableCollection<WeightData> data = [];

    [ObservableProperty]
    private ObservableCollection<WeightData> visibleMeasurements = [];

    [ObservableProperty]
    private bool isLoadingMore;

    public bool HasMoreMeasurements => nextPageStartIndex < allMeasurements.Count;

    [ObservableProperty]
    private string lastWeightReading = "--";

    [ObservableProperty]
    private string averageWeightReading = "--";

    [ObservableProperty]
    public double minWeightWithPadding;

    [ObservableProperty]
    public double maxWeightWithPadding;

    public WeightChartPageModel(DatabaseService databaseService, ILogger<WeightChartPageModel> logger)
    {
        _selectedRange = RangeType.Week;
        this.databaseService = databaseService;
        this.logger = logger;
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
                allMeasurements = filteredData.OrderByDescending(x => x.Timestamp).ToList();
                ResetVisibleMeasurements();
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Loading weight data failed");

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

    [RelayCommand]
    private async Task DeleteMeasurement(WeightData entry)
    {
        var confirm = await Shell.Current.DisplayAlertAsync(
            AppResources.displayalert_want_to_delete, $"{entry.Timestamp:dd.MM.yyyy HH:mm}",
            AppResources.button_yes, AppResources.button_no);

        if (!confirm) return;

        try
        {
            await databaseService.DeleteAsync(entry);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Deleting weight entry failed");
            await Shell.Current.DisplayAlertAsync(AppResources.displayalert_delete_failed, AppResources.displayalert_error_message, AppResources.button_ok);
            return;
        }

        // Remove locally so the list keeps its scroll position; reassigning Data refreshes chart and summary
        allMeasurements.Remove(entry);
        VisibleMeasurements.Remove(entry);
        nextPageStartIndex--;
        OnPropertyChanged(nameof(HasMoreMeasurements));

        Data = new ObservableCollection<WeightData>(Data.Where(d => d.Id != entry.Id));
    }

    private void ResetVisibleMeasurements()
    {
        // Assign the first page in one go: replacing the collection and then adding items one by one
        // makes the iOS CollectionView lay out the items over the header (chart) while it reloads.
        VisibleMeasurements = new ObservableCollection<WeightData>(allMeasurements.Take(PageSize));
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
            var avgWeight = Math.Round(Data.Average(d => d.Weight), 1);
            AverageWeightReading = $"{avgWeight}";

            var latestWeight = Math.Round(Data.OrderByDescending(d => d.Timestamp).First().Weight, 1);
            LastWeightReading = $"{latestWeight}";

            MinWeightWithPadding = (int)Data.Min(d => d.Weight) - 2;
            MaxWeightWithPadding = (int)Data.Max(d => d.Weight) + 2;
        }
        else
        {
            AverageWeightReading = "--";
            LastWeightReading = "--";
        }
    }


    [RelayCommand]
    public async Task GoToInsertWeight()
    {
        await Shell.Current.GoToAsync(nameof(Pages.WeightInsertPage));
    }

    [RelayCommand]
    private async Task EditMeasurement(WeightData entry)
    {
        await Shell.Current.GoToAsync(nameof(Pages.WeightInsertPage), new Dictionary<string, object> { ["Entry"] = entry });
    }
}
