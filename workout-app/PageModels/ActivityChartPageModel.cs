using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace workout_app.PageModels;

public partial class ActivityChartPageModel : ObservableObject
{
    private readonly DatabaseService databaseService;
    private bool isLoading;
    private RangeType selectedRange;
    private List<ActivityData> cachedFiltered = [];
    private List<ActivityType> cachedTop3 = [];

    // Per-activity-type chart data (top 3)
    public ObservableCollection<ActivitySummaryPoint> Activity1Data { get; } = [];
    public ObservableCollection<ActivitySummaryPoint> Activity2Data { get; } = [];
    public ObservableCollection<ActivitySummaryPoint> Activity3Data { get; } = [];

    // Checkbox toggles
    [ObservableProperty] private bool showActivity1 = true;
    [ObservableProperty] private bool showActivity2 = true;
    [ObservableProperty] private bool showActivity3 = true;

    // Dynamic labels for the top 3 activity types
    [ObservableProperty] private string activity1Label = "";
    [ObservableProperty] private string activity2Label = "";
    [ObservableProperty] private string activity3Label = "";

    // Whether each slot has data (for hiding empty checkboxes)
    [ObservableProperty] private bool hasActivity1;
    [ObservableProperty] private bool hasActivity2;
    [ObservableProperty] private bool hasActivity3;

    // Summary
    [ObservableProperty] private string averageDistanceReading = "--";
    [ObservableProperty] private string lastDistanceReading = "--";
    [ObservableProperty] private string averageDurationReading = "--";
    [ObservableProperty] private string lastDurationReading = "--";

    public ActivityChartPageModel(DatabaseService databaseService)
    {
        this.databaseService = databaseService;
        selectedRange = RangeType.Week;
    }

    public Task LoadAsync() => LoadDataAsync();

    public RangeType SelectedRange
    {
        get => selectedRange;
        set
        {
            if (SetProperty(ref selectedRange, value))
                _ = LoadDataAsync();
        }
    }

    public ICommand SelectRangeCommand => new Command<string>(range =>
    {
        if (Enum.TryParse(range, out RangeType selected))
            SelectedRange = selected;
    });

    private async Task LoadDataAsync()
    {
        try
        {
            isLoading = true;
            var (from, to) = GetDateRange();

            var filtered = await databaseService.GetActivitiesAsync(from, to);

            // Determine top 3 activity types by occurrence count
            var top3Types = filtered
                .GroupBy(a => a.Type)
                .OrderByDescending(g => g.Count())
                .Take(3)
                .Select(g => g.Key)
                .ToList();

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                cachedFiltered = filtered;
                cachedTop3 = top3Types;

                Activity1Label = top3Types.Count > 0 ? top3Types[0].ToString() : "";
                Activity2Label = top3Types.Count > 1 ? top3Types[1].ToString() : "";
                Activity3Label = top3Types.Count > 2 ? top3Types[2].ToString() : "";

                HasActivity1 = top3Types.Count > 0;
                HasActivity2 = top3Types.Count > 1;
                HasActivity3 = top3Types.Count > 2;

                SplitData(filtered, top3Types, from, to);
                UpdateSummary();
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading activity data: {ex.Message}");
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Activity1Data.Clear();
                Activity2Data.Clear();
                Activity3Data.Clear();
            });
        }
        finally
        {
            isLoading = false;
        }
    }

    private void SplitData(List<ActivityData> activities, List<ActivityType> top3, DateTime from, DateTime to)
    {
        Activity1Data.Clear();
        Activity2Data.Clear();
        Activity3Data.Clear();

        var lookup = activities
            .GroupBy(a => a.Timestamp.Date)
            .ToDictionary(g => g.Key, g => g.ToList());

        for (var day = from; day < to; day = day.AddDays(1))
        {
            lookup.TryGetValue(day, out var dayActivities);
            dayActivities ??= [];

            var dateLabel = day.ToString("dd.MM");

            if (top3.Count > 0)
                Activity1Data.Add(new ActivitySummaryPoint
                {
                    Label = dateLabel,
                    Value = dayActivities.Where(a => a.Type == top3[0]).Sum(a => a.Duration)
                });

            if (top3.Count > 1)
                Activity2Data.Add(new ActivitySummaryPoint
                {
                    Label = dateLabel,
                    Value = dayActivities.Where(a => a.Type == top3[1]).Sum(a => a.Duration)
                });

            if (top3.Count > 2)
                Activity3Data.Add(new ActivitySummaryPoint
                {
                    Label = dateLabel,
                    Value = dayActivities.Where(a => a.Type == top3[2]).Sum(a => a.Duration)
                });
        }
    }

    partial void OnShowActivity1Changed(bool value) => UpdateSummary();
    partial void OnShowActivity2Changed(bool value) => UpdateSummary();
    partial void OnShowActivity3Changed(bool value) => UpdateSummary();

    private void UpdateSummary()
    {
        var selectedTypes = new List<ActivityType>();
        if (ShowActivity1 && cachedTop3.Count > 0) selectedTypes.Add(cachedTop3[0]);
        if (ShowActivity2 && cachedTop3.Count > 1) selectedTypes.Add(cachedTop3[1]);
        if (ShowActivity3 && cachedTop3.Count > 2) selectedTypes.Add(cachedTop3[2]);

        var activities = cachedFiltered
            .Where(a => selectedTypes.Contains(a.Type))
            .ToList();

        if (activities.Count > 0)
        {
            var avg = Math.Round(activities.Average(a => a.Distance), 1);
            AverageDistanceReading = $"{avg}";

            var avgDur = Math.Round(activities.Average(a => a.Duration), 1);
            AverageDurationReading = $"{avgDur}";

            var last = activities.OrderByDescending(a => a.Timestamp).First();
            LastDistanceReading = $"{Math.Round(last.Distance, 1)}";
            LastDurationReading = last.Duration.ToString();
        }
        else
        {
            AverageDistanceReading = "--";
            LastDistanceReading = "--";
            AverageDurationReading = "--";
            LastDurationReading = "--";
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

    [RelayCommand]
    public async Task GoToInsertActivity()
    {
        await Shell.Current.GoToAsync(nameof(Pages.ActivityInsertPage));
    }
}

