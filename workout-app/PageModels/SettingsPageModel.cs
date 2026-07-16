using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.LocalNotification;
using Plugin.LocalNotification.Core.Models;
using System.Text;

namespace workout_app.PageModels;

public partial class SettingsPageModel : ObservableObject
{
    private readonly DatabaseService _db;

    [ObservableProperty] private string appVersion = AppInfo.Current.VersionString;
    [ObservableProperty] private bool isLoggingEnabled;
    [ObservableProperty] private string statusMessage = "";
    [ObservableProperty] private TimeSpan notificationTime;
    [ObservableProperty] private bool isNotificationEnabled;

    public SettingsPageModel(DatabaseService db)
    {
        _db = db;
        LoadSettings();
    }

    private void LoadSettings()
    {
        IsLoggingEnabled = Preferences.Get("IsLoggingEnabled", false);

        if (Preferences.ContainsKey("NotificationTimeTicks"))
            NotificationTime = new TimeSpan(Preferences.Get("NotificationTimeTicks", TimeSpan.Zero.Ticks));

        if (Preferences.ContainsKey("IsNotificationEnabled"))
            IsNotificationEnabled = Preferences.Get("IsNotificationEnabled", false);
    }

    partial void OnIsLoggingEnabledChanged(bool value)
    {
        Preferences.Set("IsLoggingEnabled", value);
        SerilogConfig.ConfigureLogging();
    }

    partial void OnNotificationTimeChanged(TimeSpan value)
    {
        var saved = new TimeSpan(Preferences.Get("NotificationTimeTicks", TimeSpan.Zero.Ticks));
        if (saved == value) return;

        Preferences.Set("NotificationTimeTicks", value.Ticks);
        if (IsNotificationEnabled)
            _ = ScheduleNotificationAsync();
    }

    [RelayCommand]
    public async Task ToggleNotification(bool isToggled)
    {
        Preferences.Set("IsNotificationEnabled", isToggled);

        if (isToggled)
            await ScheduleNotificationAsync();
        else
            LocalNotificationCenter.Current.Cancel(1337);
    }

    private async Task ScheduleNotificationAsync()
    {
        if (!await LocalNotificationCenter.Current.AreNotificationsEnabled())
            await LocalNotificationCenter.Current.RequestNotificationPermission();

        LocalNotificationCenter.Current.Cancel(1337);

        var now = DateTime.Now;
        var todayAt = new DateTime(now.Year, now.Month, now.Day,
            NotificationTime.Hours, NotificationTime.Minutes, NotificationTime.Seconds);
        var notifyTime = todayAt <= now ? todayAt.AddDays(1) : todayAt;

        var request = new NotificationRequest
        {
            NotificationId = 1337,
            Title = "Workout Reminder",
            Description = "Time to log your workout!",
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = notifyTime,
                RepeatType = NotificationRepeat.Daily
            }
        };

        await LocalNotificationCenter.Current.Show(request);
    }

    [RelayCommand]
    public async Task ExportDataToCsv()
    {
        try
        {
            StatusMessage = "Exporting...";
            var folder = FileSystem.AppDataDirectory;
            var filesToShare = new List<ShareFile>();

            // Weight
            var weights = await _db.GetWeightsAsync(DateTime.MinValue, DateTime.MaxValue);
            var weightCsv = new StringBuilder();
            weightCsv.AppendLine("Id,Timestamp,Weight");
            foreach (var w in weights)
                weightCsv.AppendLine($"{w.Id},{w.Timestamp:yyyy-MM-dd HH:mm:ss},{w.Weight}");
            var weightPath = Path.Combine(folder, "weight.csv");
            await File.WriteAllTextAsync(weightPath, weightCsv.ToString());
            filesToShare.Add(new ShareFile(weightPath, "text/csv"));

            // Blood Pressure
            var bpData = await _db.GetCardioAsync(DateTime.MinValue, DateTime.MaxValue);
            var bpCsv = new StringBuilder();
            bpCsv.AppendLine("Id,Timestamp,Systolic,Diastolic,HeartRate");
            foreach (var bp in bpData)
                bpCsv.AppendLine($"{bp.Id},{bp.Timestamp:yyyy-MM-dd HH:mm:ss},{bp.Systolic},{bp.Diastolic},{bp.HeartRate}");
            var bpPath = Path.Combine(folder, "bloodpressure.csv");
            await File.WriteAllTextAsync(bpPath, bpCsv.ToString());
            filesToShare.Add(new ShareFile(bpPath, "text/csv"));

            // Activity
            var activities = await _db.GetActivitiesAsync(DateTime.MinValue, DateTime.MaxValue);
            var actCsv = new StringBuilder();
            actCsv.AppendLine("Id,Timestamp,Type,Duration,Distance,Altitude,Description");
            foreach (var a in activities)
                actCsv.AppendLine($"{a.Id},{a.Timestamp:yyyy-MM-dd HH:mm:ss},{a.Type},{a.Duration},{a.Distance},{a.Altitude},{a.Description}");
            var actPath = Path.Combine(folder, "activity.csv");
            await File.WriteAllTextAsync(actPath, actCsv.ToString());
            filesToShare.Add(new ShareFile(actPath, "text/csv"));

            await Share.Default.RequestAsync(new ShareMultipleFilesRequest
            {
                Title = "Export Workout Data",
                Files = filesToShare
            });
            StatusMessage = "Export complete.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Export failed: {ex.Message}";
        }
    }

    [RelayCommand]
    public async Task SendLogs()
    {
        try
        {
            var logFiles = Directory.GetFiles(FileSystem.AppDataDirectory, "log-*.log")
                .Select(f => new ShareFile(f, "text/plain"))
                .ToList();

            if (logFiles.Count == 0)
            {
                StatusMessage = "No log files found.";
                return;
            }

            await Share.Default.RequestAsync(new ShareMultipleFilesRequest
            {
                Title = "Send Logs",
                Files = logFiles
            });
            //StatusMessage = "";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to send logs: {ex.Message}";
        }
    }
}
