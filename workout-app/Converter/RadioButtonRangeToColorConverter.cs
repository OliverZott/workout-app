using System.Globalization;

namespace workout_app.Converter;

public class RadioButtonRangeToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is RangeType selected && parameter is string target &&
            Enum.TryParse(target, out RangeType targetRange))
        {
            return selected == targetRange ? Color.FromArgb("#edf3fd") : Colors.White;
        }
        return Colors.LightGray;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}