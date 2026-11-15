using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text;

namespace workout_app.PageModels;

public partial class SettingsPageModel : ObservableObject
{
    private readonly DatabaseService _db;
    private readonly ILogger<SettingsPageModel> _logger;
    private readonly MedicationReminderService _medicationReminders;
    private readonly WorkoutReminderService _workoutReminder;

    [ObservableProperty] private string appVersion = AppInfo.Current.VersionString;
    [ObservableProperty] private bool isMedicationReminderEnabled;
    [ObservableProperty] private bool isLoggingEnabled;
    [ObservableProperty] private string statusMessage = "";
    [ObservableProperty] private TimeSpan notificationTime;
    [ObservableProperty] private bool isNotificationEnabled;

    public SettingsPageModel(DatabaseService db, ILogger<SettingsPageModel> logger,
        MedicationReminderService medicationReminders, WorkoutReminderService workoutReminder)
    {
        _db = db;
        _logger = logger;
        _medicationReminders = medicationReminders;
        _workoutReminder = workoutReminder;
        LoadSettings();
    }

    private void LoadSettings()
    {
        IsLoggingEnabled = Preferences.Get("IsLoggingEnabled", false);

        NotificationTime = _workoutReminder.Time;
        IsNotificationEnabled = _workoutReminder.IsEnabled;
        IsMedicationReminderEnabled = _medicationReminders.IsEnabled;
    }

    async partial void OnIsMedicationReminderEnabledChanged(bool value)
    {
        // Ignores the initial load and the revert below
        if (value == _medicationReminders.IsEnabled) return;

        if (value && !await NotificationPermission.RequestAsync())
        {
            IsMedicationReminderEnabled = false;
            return;
        }

        _medicationReminders.IsEnabled = value;
        await _medicationReminders.RescheduleAsync();
    }

    partial void OnIsLoggingEnabledChanged(bool value)
    {
        Preferences.Set("IsLoggingEnabled", value);
        SerilogConfig.SetDetailedFileLogging(value);
    }

    async partial void OnNotificationTimeChanged(TimeSpan value)
    {
        if (value == _workoutReminder.Time) return;

        _workoutReminder.Time = value;
        if (IsNotificationEnabled)
            await _workoutReminder.RescheduleAsync();
    }

    async partial void OnIsNotificationEnabledChanged(bool value)
    {
        // Ignores the initial load and the revert below
        if (value == _workoutReminder.IsEnabled) return;

        if (value && !await NotificationPermission.RequestAsync())
        {
            IsNotificationEnabled = false;
            return;
        }

        _workoutReminder.IsEnabled = value;
        await _workoutReminder.RescheduleAsync();
    }

    [RelayCommand]
    public async Task ExportDataToCsv()
    {
        try
        {
            StatusMessage = AppResources.status_exporting;
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

            // Medication (name/dosage are free text and may contain commas, so they are quoted)
            static string Quote(string s) => $"\"{s.Replace("\"", "\"\"")}\"";
            var medications = await _db.GetMedicationsAsync();
            var medCsv = new StringBuilder();
            medCsv.AppendLine("Id,Name,Dosage,Fasting,Morning,Noon,Evening,Night,IsInactive");
            foreach (var m in medications)
                medCsv.AppendLine($"{m.Id},{Quote(m.Name)},{Quote(m.Dosage)},{m.Fasting},{m.Morning},{m.Noon},{m.Evening},{m.Night},{m.IsInactive}");
            var medPath = Path.Combine(folder, "medication.csv");
            await File.WriteAllTextAsync(medPath, medCsv.ToString());
            filesToShare.Add(new ShareFile(medPath, "text/csv"));

            await Share.Default.RequestAsync(new ShareMultipleFilesRequest
            {
                Title = AppResources.share_export_workout_data_title,
                Files = filesToShare
            });
            StatusMessage = AppResources.status_export_complete;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CSV export failed");
            StatusMessage = $"{AppResources.status_export_failed_prefix}: {ex.Message}";
        }
    }

    [RelayCommand]
    public async Task ExportDatabaseBackup()
    {
        try
        {
            StatusMessage = AppResources.status_exporting;

            var backupPath = await _db.CreateDatabaseBackupAsync();
            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = AppResources.share_export_db_title,
                File = new ShareFile(backupPath, "application/octet-stream")
            });

            StatusMessage = AppResources.status_db_export_complete;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database export failed");
            StatusMessage = $"{AppResources.status_db_export_failed_prefix}: {ex.Message}";
        }
    }

    [RelayCommand]
    public async Task ImportDatabaseBackup()
    {
        try
        {
            var confirmReplace = await Shell.Current.DisplayAlertAsync(
                AppResources.settings_db_import_confirm_title,
                AppResources.settings_db_import_confirm_message,
                AppResources.button_yes,
                AppResources.button_no);

            if (!confirmReplace)
                return;

            var pickedFile = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = AppResources.settings_db_import_picker_title
            });

            if (pickedFile is null)
                return;

            if (!string.IsNullOrWhiteSpace(pickedFile.FullPath))
            {
                await _db.ReplaceDatabaseFromFileAsync(pickedFile.FullPath);
            }
            else
            {
                var tempImportPath = Path.Combine(FileSystem.CacheDirectory, $"import-{Guid.NewGuid():N}.db3");
                await using var pickedStream = await pickedFile.OpenReadAsync();
                await using var targetStream = File.Create(tempImportPath);
                await pickedStream.CopyToAsync(targetStream);
                await targetStream.FlushAsync();

                await _db.ReplaceDatabaseFromFileAsync(tempImportPath);
            }

            StatusMessage = AppResources.status_db_import_complete;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database import failed");
            StatusMessage = $"{AppResources.status_db_import_failed_prefix}: {ex.Message}";
        }
    }

    [RelayCommand]
    public async Task SendLogs()
    {
        try
        {
            var logFiles = Directory.GetFiles(SerilogConfig.LogDirectory, SerilogConfig.LogFilePattern)
                .Order()
                .Select(f => new ShareFile(f, "text/plain"))
                .ToList();

            if (logFiles.Count == 0)
            {
                StatusMessage = AppResources.status_no_log_files;
                return;
            }

            await Share.Default.RequestAsync(new ShareMultipleFilesRequest
            {
                Title = AppResources.share_send_logs_title,
                Files = logFiles
            });
            //StatusMessage = "";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sending log files failed");
            StatusMessage = $"{AppResources.status_send_logs_failed_prefix}: {ex.Message}";
        }
    }
}
