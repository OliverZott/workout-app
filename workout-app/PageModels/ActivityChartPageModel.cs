using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace workout_app.PageModels;

public partial class ActivityChartPageModel : ObservableObject
{
    private readonly DatabaseService databaseService;
    private bool isLoading;
    private RangeType selectedRange;

    [ObservableProperty]
    private ObservableCollection<ActivitySummaryPoint> chartData = [];

    [ObservableProperty]
    private ActivityType selectedActivityType = ActivityType.Hiking;

    public IReadOnlyList<ActivityType> AvailableActivities { get; } =
        Enum.GetValues<ActivityType>().ToList();

    public ActivityChartPageModel(DatabaseService databaseService)
    {
        this.databaseService = databaseService;
        selectedRange = RangeType.Week;
        _ = LoadDataAsync();
    }

    public Task RefreshAsync() => LoadDataAsync();

    public RangeType SelectedRange
    {
        get => selectedRange;
        set
        {
            if (SetProperty(ref selectedRange, value))
            {
                _ = LoadDataAsync();
            }
        }
    }

    partial void OnSelectedActivityTypeChanged(ActivityType value)
    {
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            isLoading = true;

            var (start, end) = GetDateRange();

            var allActivities = await databaseService.GetActivitiesAsync();

            var filtered = allActivities
                .Where(a => a.Type == SelectedActivityType &&
                            a.Timestamp.Date > start.Date &&
                            a.Timestamp.Date <= end.Date)
                .GroupBy(a => a.Timestamp.Date)
                .OrderBy(g => g.Key)
                .Select(g => new ActivitySummaryPoint
                {
                    Label = g.Key.ToString("dd.MM"),
                    // For now we chart total distance per day; easily switch to altitude or duration later.
                    Value = g.Sum(x => x.Distance)
                })
                .ToList();

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                ChartData = new ObservableCollection<ActivitySummaryPoint>(filtered);
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading activity data: {ex.Message}");

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                ChartData = [];
            });
        }
        finally
        {
            isLoading = false;
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
}

