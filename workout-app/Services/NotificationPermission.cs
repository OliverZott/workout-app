using Plugin.LocalNotification;

namespace workout_app.Services;

public static class NotificationPermission
{
    // Asks only if not granted yet; returns whether notifications may be shown
    public static async Task<bool> RequestAsync() =>
        await LocalNotificationCenter.Current.AreNotificationsEnabled()
        || await LocalNotificationCenter.Current.RequestNotificationPermission();
}
