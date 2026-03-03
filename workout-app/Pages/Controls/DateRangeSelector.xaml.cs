using System.Windows.Input;

namespace workout_app.Pages.Controls;

public partial class DateRangeSelector : ContentView
{
    public static readonly BindableProperty SelectedRangeProperty =
       BindableProperty.Create(nameof(SelectedRange), typeof(RangeType), typeof(DateRangeSelector), RangeType.Week, BindingMode.TwoWay);

    public static readonly BindableProperty RangeSelectedCommandProperty =
        BindableProperty.Create(nameof(RangeSelectedCommand), typeof(ICommand), typeof(DateRangeSelector));

    public static readonly BindableProperty FirstRangeTextProperty =
        BindableProperty.Create(nameof(FirstRangeText), typeof(string), typeof(DateRangeSelector), "Woche");

    public static readonly BindableProperty SecondRangeTextProperty =
        BindableProperty.Create(nameof(SecondRangeText), typeof(string), typeof(DateRangeSelector), "Monat");

    public static readonly BindableProperty ThirdRangeTextProperty =
        BindableProperty.Create(nameof(ThirdRangeText), typeof(string), typeof(DateRangeSelector), "Max");

    public static readonly BindableProperty FirstRangeValueProperty =
        BindableProperty.Create(nameof(FirstRangeValue), typeof(string), typeof(DateRangeSelector), "Week");

    public static readonly BindableProperty SecondRangeValueProperty =
        BindableProperty.Create(nameof(SecondRangeValue), typeof(string), typeof(DateRangeSelector), "Month");

    public static readonly BindableProperty ThirdRangeValueProperty =
        BindableProperty.Create(nameof(ThirdRangeValue), typeof(string), typeof(DateRangeSelector), "Max");

    public RangeType SelectedRange
    {
        get => (RangeType)GetValue(SelectedRangeProperty);
        set => SetValue(SelectedRangeProperty, value);
    }

    public ICommand RangeSelectedCommand
    {
        get => (ICommand)GetValue(RangeSelectedCommandProperty);
        set => SetValue(RangeSelectedCommandProperty, value);
    }

    public string FirstRangeText
    {
        get => (string)GetValue(FirstRangeTextProperty);
        set => SetValue(FirstRangeTextProperty, value);
    }

    public string SecondRangeText
    {
        get => (string)GetValue(SecondRangeTextProperty);
        set => SetValue(SecondRangeTextProperty, value);
    }

    public string ThirdRangeText
    {
        get => (string)GetValue(ThirdRangeTextProperty);
        set => SetValue(ThirdRangeTextProperty, value);
    }

    public string FirstRangeValue
    {
        get => (string)GetValue(FirstRangeValueProperty);
        set => SetValue(FirstRangeValueProperty, value);
    }

    public string SecondRangeValue
    {
        get => (string)GetValue(SecondRangeValueProperty);
        set => SetValue(SecondRangeValueProperty, value);
    }

    public string ThirdRangeValue
    {
        get => (string)GetValue(ThirdRangeValueProperty);
        set => SetValue(ThirdRangeValueProperty, value);
    }

    public DateRangeSelector()
    {
        InitializeComponent();
    }
}