using Android.Content;
using Android.Hardware;

namespace workout_app;

public record MotionSample(long TimestampNs, float X, float Y, float Z);

// Records accelerometer (m/s²) and gyroscope (rad/s) with the sensor's own timestamps,
// which MAUI's Accelerometer/Gyroscope API does not expose.
public class NativeMotionRecorder : Java.Lang.Object, ISensorEventListener
{
    private readonly SensorManager sensorManager =
        (SensorManager)Platform.AppContext.GetSystemService(Context.SensorService)!;

    public List<MotionSample> Accelerometer { get; } = [];
    public List<MotionSample> Gyroscope { get; } = [];

    public void Clear()
    {
        Accelerometer.Clear();
        Gyroscope.Clear();
    }

    public void Start(int samplingPeriodUs)
    {
        // Android accepts a period in µs here; the .NET binding just types it as SensorDelay.
        var period = (SensorDelay)samplingPeriodUs;
        sensorManager.RegisterListener(this, sensorManager.GetDefaultSensor(SensorType.Accelerometer), period);
        sensorManager.RegisterListener(this, sensorManager.GetDefaultSensor(SensorType.Gyroscope), period);
    }

    public void Stop() => sensorManager.UnregisterListener(this);

    public static void Beep()
    {
        var tone = new Android.Media.ToneGenerator(Android.Media.Stream.Notification, 100);
        tone.StartTone(Android.Media.Tone.PropBeep, 300);
    }

    // Called on the main thread, so the lists need no locking.
    public void OnSensorChanged(SensorEvent? e)
    {
        if (e?.Sensor is null || e.Values is null)
            return;

        var sample = new MotionSample(e.Timestamp, e.Values[0], e.Values[1], e.Values[2]);
        if (e.Sensor.Type == SensorType.Accelerometer)
            Accelerometer.Add(sample);
        else
            Gyroscope.Add(sample);
    }

    public void OnAccuracyChanged(Sensor? sensor, SensorStatus accuracy)
    {
    }
}
