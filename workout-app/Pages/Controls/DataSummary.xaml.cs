namespace workout_app.Pages.Controls;

public partial class DataSummary : ContentView
{
    public static readonly BindableProperty AverageReadingProperty =
        BindableProperty.Create(nameof(AverageReading), typeof(string), typeof(DataSummary), "--");

    public static readonly BindableProperty LastReadingProperty =
        BindableProperty.Create(nameof(LastReading), typeof(string), typeof(DataSummary), "--");

    public static readonly BindableProperty UnitProperty =
    BindableProperty.Create(nameof(Unit), typeof(string), typeof(DataSummary), "");

    public string AverageReading
    {
        get => (string)GetValue(AverageReadingProperty);
        set => SetValue(AverageReadingProperty, value);
    }

    public string LastReading
    {
        get => (string)GetValue(LastReadingProperty);
        set => SetValue(LastReadingProperty, value);
    }

    public string Unit
    {
        get => (string)GetValue(UnitProperty);
        set => SetValue(UnitProperty, value);
    }
    public DataSummary()
    {
        InitializeComponent();
    }
}