using Plugin.LocalNotification;
using Plugin.LocalNotification.Core.Models;

namespace workout_app.Services;

// Schedules one daily notification per time slot listing all medications due then.
// Always cancels and re-creates its own notifications, so callers just call RescheduleAsync after a change.
public class MedicationReminderService(DatabaseService database, ILogger<MedicationReminderService> logger)
{
    private const string EnabledKey = "IsMedicationReminderEnabled";

    // Fixed intake times for now. "Fasting" is only an intake hint and therefore has no reminder of its own.
    // IDs must not collide with the workout reminder (1337).
    private static readonly (int Id, Func<string> Name, TimeSpan Time, Func<MedicationData, int> Dose)[] Slots =
    [
        (101, () => AppResources.medication_slot_morning, new TimeSpan(8, 0, 0), m => m.Morning),
        (102, () => AppResources.medication_slot_noon, new TimeSpan(12, 0, 0), m => m.Noon),
        (103, () => AppResources.medication_slot_evening, new TimeSpan(18, 0, 0), m => m.Evening),
        (104, () => AppResources.medication_slot_night, new TimeSpan(20, 0, 0), m => m.Night),
    ];

    public bool IsEnabled
    {
        get => Preferences.Get(EnabledKey, false);
        set => Preferences.Set(EnabledKey, value);
    }

    public async Task<bool> RequestPermissionAsync() =>
        await LocalNotificationCenter.Current.AreNotificationsEnabled()
        || await LocalNotificationCenter.Current.RequestNotificationPermission();

    // Catches itself because it is also called fire-and-forget, where exceptions would go unnoticed
    public async Task RescheduleAsync()
    {
        try
        {
            foreach (var slot in Slots)
                LocalNotificationCenter.Current.Cancel(slot.Id);

            if (!IsEnabled)
                return;

            var plan = await database.GetMedicationsAsync();

            foreach (var slot in Slots)
            {
                var lines = plan
                    .Where(m => !m.IsInactive && slot.Dose(m) > 0)
                    .Select(m => $"{slot.Dose(m)}× {m.Name} {m.Dosage}".Trim())
                    .ToList();

                if (lines.Count == 0)
                    continue;

                await LocalNotificationCenter.Current.Show(new NotificationRequest
                {
                    NotificationId = slot.Id,
                    Title = string.Format(AppResources.notification_medication_title, slot.Name()),
                    Description = string.Join("\n", lines),
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = NextOccurrence(slot.Time),
                        RepeatType = NotificationRepeat.Daily
                    }
                });
            }

            logger.LogInformation("Medication reminders rescheduled for {Count} medications", plan.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Scheduling the medication reminders failed");
        }
    }

    private static DateTime NextOccurrence(TimeSpan time)
    {
        var todayAt = DateTime.Today + time;
        return todayAt <= DateTime.Now ? todayAt.AddDays(1) : todayAt;
    }
}
