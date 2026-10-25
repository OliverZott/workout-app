using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace workout_app.PageModels;

public record PlotPoint(double TimeS, double Value);

public record AxisPlot(string Title, Brush Color, List<PlotPoint> Points);

public partial class MotionSensorPageModel : ObservableObject
{
    private const int SamplingRateHz = 100;
    private static readonly TimeSpan StartDelay = TimeSpan.FromSeconds(3);
    private static readonly TimeSpan RecordingDuration = TimeSpan.FromSeconds(30);
    private const double ExpectedIntervalMs = 1000.0 / SamplingRateHz;

#if ANDROID
    private readonly NativeMotionRecorder recorder = new();
#endif
    private CancellationTokenSource? trackingCancellation;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanStart))]
    private bool isTracking;

    [ObservableProperty] private string sessionStatus = string.Format(AppResources.motion_status_ready, SamplingRateHz);
    [ObservableProperty] private string accelerometerText = AppResources.motion_no_data;
    [ObservableProperty] private string gyroscopeText = AppResources.motion_no_data;
    [ObservableProperty] private List<AxisPlot> plots = [];

    public bool CanStart => !IsTracking;

    [RelayCommand]
    public async Task StartTrackingAsync()
    {
#if ANDROID
        if (IsTracking)
            return;

        trackingCancellation = new CancellationTokenSource();
        recorder.Clear();
        Plots = [];
        IsTracking = true;
        SessionStatus = string.Format(AppResources.motion_status_countdown, StartDelay.TotalSeconds);

        try
        {
            // Time to put the phone down and take the hand away.
            await Task.Delay(StartDelay, trackingCancellation.Token);

            recorder.Start(1_000_000 / SamplingRateHz);
            trackingCancellation.CancelAfter(RecordingDuration);
            SessionStatus = string.Format(AppResources.motion_status_running, SamplingRateHz, RecordingDuration.TotalSeconds);

            while (true)
            {
                AccelerometerText = Latest(recorder.Accelerometer);
                GyroscopeText = Latest(recorder.Gyroscope);
                await Task.Delay(250, trackingCancellation.Token);
            }
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            recorder.Stop();
            IsTracking = false;
            trackingCancellation.Dispose();
            trackingCancellation = null;
        }

        NativeMotionRecorder.Beep();
        SessionStatus = AppResources.motion_status_finished;
        AccelerometerText = Stats(recorder.Accelerometer);
        GyroscopeText = Stats(recorder.Gyroscope);
        Plots = BuildPlots(recorder.Accelerometer, recorder.Gyroscope);
#else
        SessionStatus = AppResources.motion_status_android_only;
        await Task.CompletedTask;
#endif
    }

    [RelayCommand]
    public void StopTracking() => trackingCancellation?.Cancel();

    [RelayCommand]
    public async Task ExportAsync()
    {
#if ANDROID
        var stamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        await Share.Default.RequestAsync(new ShareMultipleFilesRequest
        {
            Title = AppResources.share_motion_title,
            Files =
            [
                new ShareFile(WriteCsv($"accelerometer_{stamp}.csv", recorder.Accelerometer)),
                new ShareFile(WriteCsv($"gyroscope_{stamp}.csv", recorder.Gyroscope)),
            ],
        });
#else
        await Task.CompletedTask;
#endif
    }

#if ANDROID
    private static string WriteCsv(string fileName, List<MotionSample> samples)
    {
        var path = Path.Combine(FileSystem.CacheDirectory, fileName);
        var rows = samples.Select(s => string.Create(CultureInfo.InvariantCulture, $"{s.TimestampNs},{s.X},{s.Y},{s.Z}"));
        File.WriteAllLines(path, rows.Prepend("sensorTs_ns,x,y,z"));
        return path;
    }

    private static List<AxisPlot> BuildPlots(List<MotionSample> acc, List<MotionSample> gyro)
    {
        if (acc.Count == 0 || gyro.Count == 0)
            return [];

        // Common time zero so both sensors share the same x-axis.
        var t0 = Math.Min(acc[0].TimestampNs, gyro[0].TimestampNs);
        Brush blue = Color.FromArgb("#1f77b4"), orange = Color.FromArgb("#ff7f0e"), green = Color.FromArgb("#2ca02c");

        AxisPlot Plot(string title, Brush color, List<MotionSample> samples, Func<MotionSample, float> axis) =>
            new(title, color, samples.Select(s => new PlotPoint((s.TimestampNs - t0) / 1e9, axis(s))).ToList());

        return
        [
            Plot("acc x [m/s²]", blue, acc, s => s.X),
            Plot("acc y [m/s²]", orange, acc, s => s.Y),
            Plot("acc z [m/s²]", green, acc, s => s.Z),
            Plot("gyro x [rad/s]", blue, gyro, s => s.X),
            Plot("gyro y [rad/s]", orange, gyro, s => s.Y),
            Plot("gyro z [rad/s]", green, gyro, s => s.Z),
        ];
    }

    private static string Latest(List<MotionSample> samples)
    {
        if (samples.Count == 0)
            return AppResources.motion_no_data;

        var s = samples[^1];
        return $"Samples: {samples.Count}\nX: {s.X:F3}\nY: {s.Y:F3}\nZ: {s.Z:F3}";
    }

    // Equidistance check based on the sensor timestamps.
    private static string Stats(List<MotionSample> samples)
    {
        if (samples.Count < 2)
            return AppResources.motion_too_few_samples;

        var intervalsMs = new double[samples.Count - 1];
        for (var i = 1; i < samples.Count; i++)
            intervalsMs[i - 1] = (samples[i].TimestampNs - samples[i - 1].TimestampNs) / 1e6;

        var durationS = (samples[^1].TimestampNs - samples[0].TimestampNs) / 1e9;
        var mean = intervalsMs.Average();
        var sd = Math.Sqrt(intervalsMs.Average(d => (d - mean) * (d - mean)));
        var gaps = intervalsMs.Count(d => d > 1.5 * ExpectedIntervalMs);

        return string.Format(AppResources.motion_stats,
            samples.Count, durationS, intervalsMs.Length / durationS, mean, sd, intervalsMs.Min(), intervalsMs.Max(), gaps);
    }
#endif
}
