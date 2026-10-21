using System.Globalization;

namespace workout_app.Converter;

// Maps a diet rating (1 = healthy ... 5 = unhealthy) to a green-to-red color.
public class DietRatingToColorConverter : IValueConverter
{
    private static readonly Color[] RatingColors =
    [
        Color.FromArgb("#4CAF50"),
        Color.FromArgb("#9CCC65"),
        Color.FromArgb("#FFEB3B"),
        Color.FromArgb("#FF9800"),
        Color.FromArgb("#F44336")
    ];

    // Rounds averaged ratings (e.g. 2.4) to the nearest rating color; 0 or less means "no data".
    public static Color GetColor(double rating)
    {
        if (rating <= 0)
            return Colors.Transparent;

        var index = (int)Math.Round(Math.Clamp(rating, DietData.MinRating, DietData.MaxRating)) - 1;
        return RatingColors[index];
    }

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value switch
    {
        int rating => GetColor(rating),
        double rating => GetColor(rating),
        _ => Colors.LightGray
    };

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}
