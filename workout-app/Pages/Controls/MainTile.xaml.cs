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
        // Scope to the inner content only; setting it on the control itself would hide the page's view model from the consumer's bindings
        Content.BindingContext = this;
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