using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;

namespace workout_app.PageModels;

public partial class DietChartPageModel : ObservableObject
{
    private const int PageSize = 20;
    private readonly DatabaseService databaseService;
    private RangeType selectedRange;
    private DateTime periodStart;
    private List<DietData> cachedFiltered = [];
    private List<DietData> allEntries = [];
    private int nextPageStartIndex;

    // Calendar grid (Monday first); each day is colored by its average rating
    public IReadOnlyList<WeekdayHeader> WeekdayHeaders { get; } = BuildWeekdayHeaders();
    [ObservableProperty] private List<DietCalendarWeek> calendarWeeks = [];

    // Period navigation
    [ObservableProperty] private string periodLabel = "";
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(NextPeriodCommand))]
    private bool canGoNext;

    // Meal list below the calendar
    [ObservableProperty] private ObservableCollection<DietData> visibleEntries = [];
    [ObservableProperty] private bool isLoadingMore;

    public bool HasMoreEntries => nextPageStartIndex < allEntries.Count;

    // Summary
    [ObservableProperty] private string averageRatingReading = "--";
    [ObservableProperty] private string lastRatingReading = "--";

    public DietChartPageModel(DatabaseService databaseService)
    {
        this.databaseService = databaseService;
        selectedRange = RangeType.Week;
        periodStart = GetPeriodStart(DateTime.Today, selectedRange);
    }

    public Task LoadAsync() => LoadDataAsync();

    public RangeType SelectedRange
    {
        get => selectedRange;
        set
        {
            if (SetProperty(ref selectedRange, value))
            {
                // Switching the range jumps back to the current week/month
                periodStart = GetPeriodStart(DateTime.Today, value);
                _ = LoadDataAsync();
            }
        }
    }

    public ICommand SelectRangeCommand => new Command<string>(range =>
    {
        if (Enum.TryParse(range, out RangeType selected))
            SelectedRange = selected;
    });

    [RelayCommand]
    private async Task PreviousPeriod()
    {
        periodStart = ShiftPeriod(periodStart, -1);
        await LoadDataAsync();
    }

    [RelayCommand(CanExecute = nameof(CanGoNext))]
    private async Task NextPeriod()
    {
        periodStart = ShiftPeriod(periodStart, 1);
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        var (from, to) = GetDateRange();

        try
        {
            var filtered = await databaseService.GetDietAsync(from, to);

            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                cachedFiltered = filtered;
                UpdateCalendar();

                allEntries = filtered.OrderByDescending(d => d.Timestamp).ToList();
                ResetVisibleEntries();
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading diet data: {ex.Message}");
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                cachedFiltered = [];
                UpdateCalendar();
                allEntries = [];
                ResetVisibleEntries();
            });
        }
    }

    // Rebuilds calendar cells, period label and summary from cachedFiltered
    private void UpdateCalendar()
    {
        var (from, to) = GetDateRange();

        var dailyAverages = cachedFiltered
            .GroupBy(d => d.Timestamp.Date)
            .ToDictionary(g => g.Key, g => Math.Round(g.Average(d => d.Rating), 1));

        // Grid always spans full Monday-Sunday weeks; days outside the period are left empty
        var gridStart = GetWeekStart(from);
        var weeks = new List<DietCalendarWeek>();
        for (var weekStart = gridStart; weekStart < to; weekStart = weekStart.AddDays(7))
        {
            weeks.Add(new DietCalendarWeek
            {
                Days = [.. Enumerable.Range(0, 7).Select(i =>
                {
                    var day = weekStart.AddDays(i);
                    return new DietCalendarDay
                    {
                        Date = day,
                        Column = i,
                        IsInPeriod = day >= from && day < to,
                        AverageRating = dailyAverages.GetValueOrDefault(day)
                    };
                })]
            });
        }
        CalendarWeeks = weeks;

        PeriodLabel = SelectedRange == RangeType.Month
            ? from.ToString("MMMM yyyy", CultureInfo.CurrentCulture)
            : $"{from:dd.MM.} – {to.AddDays(-1):dd.MM.yyyy}";
        CanGoNext = to <= DateTime.Today;

        if (cachedFiltered.Count > 0)
        {
            AverageRatingReading = $"{Math.Round(cachedFiltered.Average(d => d.Rating), 1)}";
            LastRatingReading = cachedFiltered.MaxBy(d => d.Timestamp)!.Rating.ToString();
        }
        else
        {
            AverageRatingReading = "--";
            LastRatingReading = "--";
        }
    }

    [RelayCommand]
    private async Task DeleteEntry(DietData entry)
    {
        var confirm = await Shell.Current.DisplayAlertAsync(
            AppResources.displayalert_want_to_delete, $"{entry.MealDisplay}\n{entry.Timestamp:dd.MM.yyyy HH:mm}",
            AppResources.button_yes, AppResources.button_no);

        if (!confirm) return;

        await databaseService.DeleteAsync(entry);

        // Remove locally so the list keeps its scroll position, then rebuild calendar and summary
        allEntries.Remove(entry);
        VisibleEntries.Remove(entry);
        nextPageStartIndex--;
        OnPropertyChanged(nameof(HasMoreEntries));

        cachedFiltered.Remove(entry);
        UpdateCalendar();
    }

    [RelayCommand]
    private void LoadMoreEntries()
    {
        if (IsLoadingMore || !HasMoreEntries)
            return;

        IsLoadingMore = true;

        foreach (var item in allEntries.Skip(nextPageStartIndex).Take(PageSize))
            VisibleEntries.Add(item);

        nextPageStartIndex += PageSize;
        OnPropertyChanged(nameof(HasMoreEntries));

        IsLoadingMore = false;
    }

    private void ResetVisibleEntries()
    {
        // Assign the first page in one go: replacing the collection and then adding items one by one
        // makes the iOS CollectionView lay out the items over the header (calendar) while it reloads.
        VisibleEntries = new ObservableCollection<DietData>(allEntries.Take(PageSize));
        nextPageStartIndex = VisibleEntries.Count;
        OnPropertyChanged(nameof(HasMoreEntries));
    }

    private (DateTime from, DateTime to) GetDateRange() =>
        (periodStart, ShiftPeriod(periodStart, 1));

    private DateTime ShiftPeriod(DateTime start, int count) =>
        SelectedRange == RangeType.Month ? start.AddMonths(count) : start.AddDays(7 * count);

    private static DateTime GetPeriodStart(DateTime date, RangeType range) =>
        range == RangeType.Month ? new DateTime(date.Year, date.Month, 1) : GetWeekStart(date);

    private static DateTime GetWeekStart(DateTime date) =>
        date.Date.AddDays(-(((int)date.DayOfWeek + 6) % 7));

    private static List<WeekdayHeader> BuildWeekdayHeaders()
    {
        var names = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames;
        return [.. Enumerable.Range(0, 7).Select(i => new WeekdayHeader(names[(i + 1) % 7], i))];
    }

    [RelayCommand]
    public async Task GoToInsertDiet()
    {
        await Shell.Current.GoToAsync(nameof(Pages.DietInsertPage));
    }
}
