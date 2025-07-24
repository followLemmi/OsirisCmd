using System.Globalization;
using Avalonia.Data.Converters;

namespace OsirisCmd.UI.Converters;

public class StringListConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is List<string> list)
        {
            return string.Join(", ", list);
        }

        return "";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string str)
        {
            return new List<string>(str.Split(new[] { ',' },
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
        }
        return new List<string>();
    }
}