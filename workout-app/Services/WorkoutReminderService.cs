using Plugin.LocalNotification;
using Plugin.LocalNotification.Core.Models;

namespace workout_app.Services;

// Daily "log your workout" reminder at a user-chosen time.
// Always cancels and re-creates its notification, so callers just call RescheduleAsync after a change.
public class WorkoutReminderService(ILogger<WorkoutReminderService> logger)
{
    private const int NotificationId = 1337;
    private const string EnabledKey = "IsNotificationEnabled";
    private const string TimeKey = "NotificationTimeTicks";

    public bool IsEnabled
    {
        get => Preferences.Get(EnabledKey, false);
        set => Preferences.Set(EnabledKey, value);
    }

    public TimeSpan Time
    {
        get => new(Preferences.Get(TimeKey, TimeSpan.Zero.Ticks));
        set => Preferences.Set(TimeKey, value.Ticks);
    }

    // Catches itself because it is also called fire-and-forget, where exceptions would go unnoticed
    public async Task RescheduleAsync()
    {
        try
        {
            LocalNotificationCenter.Current.Cancel(NotificationId);

            if (!IsEnabled)
                return;

            var todayAt = DateTime.Today + Time;
            var notifyTime = todayAt <= DateTime.Now ? todayAt.AddDays(1) : todayAt;

            await LocalNotificationCenter.Current.Show(new NotificationRequest
            {
                NotificationId = NotificationId,
                Title = AppResources.notification_workout_reminder_title,
                Description = AppResources.notification_workout_reminder_description,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = notifyTime,
                    RepeatType = NotificationRepeat.Daily
                }
            });

            logger.LogInformation("Workout reminder scheduled daily from {NotifyTime}", notifyTime);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Scheduling the workout reminder failed");
        }
    }
}
