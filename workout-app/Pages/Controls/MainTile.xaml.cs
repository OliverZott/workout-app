using System.Windows.Input;

namespace workout_app.Pages.Controls;

public partial class MainTile : ContentView
{
    public static readonly BindableProperty TileTextProperty =
        BindableProperty.Create(nameof(TileText), typeof(string), typeof(MainTile), string.Empty);

    public static readonly BindableProperty ImageSourceProperty =
        BindableProperty.Create(nameof(ImageSource), typeof(string), typeof(MainTile), string.Empty);

    public static readonly BindableProperty TapCommandProperty =
        BindableProperty.Create(nameof(TapCommand), typeof(ICommand), typeof(MainTile), null);


    public MainTile()
    {
        InitializeComponent();
        BindingContext = this;
    }


    public string TileText
    {
        get => (string)GetValue(TileTextProperty);
        set => SetValue(TileTextProperty, value);
    }

    public string ImageSource
    {
        get => (string)GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }

    public ICommand TapCommand
    {
        get => (ICommand)GetValue(TapCommandProperty);
        set => SetValue(TapCommandProperty, value);
    }
}