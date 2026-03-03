namespace workout_app.Pages.Controls;

public partial class DateTimePicker : ContentView
{
    public static readonly BindableProperty SelectedDateProperty =
      BindableProperty.Create(
          nameof(SelectedDate),
          typeof(DateTime),
          typeof(DateTimePicker),
          default(DateTime),
          BindingMode.TwoWay);

    public DateTime SelectedDate
    {
        get => (DateTime)GetValue(SelectedDateProperty);
        set => SetValue(SelectedDateProperty, value);
    }

    public static readonly BindableProperty SelectedTimeProperty =
        BindableProperty.Create(
            nameof(SelectedTime),
            typeof(TimeSpan),
            typeof(DateTimePicker),
            default(TimeSpan),
            BindingMode.TwoWay);

    public TimeSpan SelectedTime
    {
        get => (TimeSpan)GetValue(SelectedTimeProperty);
        set => SetValue(SelectedTimeProperty, value);
    }

    public static readonly BindableProperty SelectedDateDisplayProperty =
        BindableProperty.Create(
            nameof(SelectedDateDisplay),
            typeof(string),
            typeof(DateTimePicker),
            default(string),
            BindingMode.TwoWay);

    public string SelectedDateDisplay
    {
        get => (string)GetValue(SelectedDateDisplayProperty);
        set => SetValue(SelectedDateDisplayProperty, value);
    }

    public static readonly BindableProperty SelectedTimeDisplayProperty =
        BindableProperty.Create(
            nameof(SelectedTimeDisplay),
            typeof(string),
            typeof(DateTimePicker),
            default(string),
            BindingMode.TwoWay);

    public string SelectedTimeDisplay
    {
        get => (string)GetValue(SelectedTimeDisplayProperty);
        set => SetValue(SelectedTimeDisplayProperty, value);
    }

    public static readonly BindableProperty ShowTimePickerProperty =
    BindableProperty.Create(
        nameof(ShowTimePicker),
        typeof(bool),
        typeof(DateTimePicker),
        true);

    public bool ShowTimePicker
    {
        get => (bool)GetValue(ShowTimePickerProperty);
        set => SetValue(ShowTimePickerProperty, value);
    }


    public DateTimePicker()
    {
        InitializeComponent();
    }
}