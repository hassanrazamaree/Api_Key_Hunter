using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using APIKeyHunter.Core.Models;

namespace APIKeyHunter.WPF.Converters;

/// <summary>Returns Visibility.Visible when the value is not null or empty.</summary>
public sealed class NullOrEmptyToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var hasValue = value switch
        {
            string s => !string.IsNullOrWhiteSpace(s),
            int i    => i > 0,
            bool b   => b,
            null     => false,
            _        => true
        };
        bool invert = parameter as string == "invert";
        return (hasValue ^ invert) ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>Converts bool to Visibility; optionally inverts.</summary>
public sealed class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool b = value is bool b2 && b2;
        bool invert = parameter as string == "invert";
        return (b ^ invert) ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is Visibility v && v == Visibility.Visible;
}

/// <summary>Inverts a bool.</summary>
public sealed class BoolNegateConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool b && !b;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool b && !b;
}

/// <summary>Converts a double percentage (0-100) to a formatted string.</summary>
public sealed class PercentageConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is double d ? $"{d:F0}%" : "0%";

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>Maps KeyStatus to a readable coloured brush.</summary>
public sealed class KeyStatusToBrushConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is KeyStatus status)
        {
            return status switch
            {
                KeyStatus.Valid       => new SolidColorBrush(Color.FromRgb(0x3F, 0xB9, 0x50)),
                KeyStatus.Invalid     => new SolidColorBrush(Color.FromRgb(0xF8, 0x51, 0x49)),
                KeyStatus.RateLimited => new SolidColorBrush(Color.FromRgb(0xD2, 0x99, 0x22)),
                KeyStatus.Expired     => new SolidColorBrush(Color.FromRgb(0x84, 0x8D, 0x97)),
                _                    => new SolidColorBrush(Color.FromRgb(0x58, 0xA6, 0xFF)),
            };
        }
        return new SolidColorBrush(Colors.Gray);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>Maps TokenTestStatus to a display brush.</summary>
public sealed class TokenStatusToBrushConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is TokenTestStatus status)
        {
            return status switch
            {
                TokenTestStatus.Valid     => new SolidColorBrush(Color.FromRgb(0x3F, 0xB9, 0x50)),
                TokenTestStatus.Invalid   => new SolidColorBrush(Color.FromRgb(0xF8, 0x51, 0x49)),
                TokenTestStatus.Testing   => new SolidColorBrush(Color.FromRgb(0x58, 0xA6, 0xFF)),
                _                        => new SolidColorBrush(Color.FromRgb(0x84, 0x8D, 0x97)),
            };
        }
        return new SolidColorBrush(Colors.Gray);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>Formats a nullable DateTime as a local string or dash.</summary>
public sealed class DateTimeConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTime dt)
            return dt.ToLocalTime().ToString("yyyy-MM-dd HH:mm", culture);
        return "—";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>Formats a ValidationStatus enum to a display string.</summary>
public sealed class ValidationStatusConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is ValidationStatus s)
        {
            return s switch
            {
                ValidationStatus.Pending    => "Pending",
                ValidationStatus.InProgress => "In Progress",
                ValidationStatus.Valid      => "Valid",
                ValidationStatus.Invalid    => "Invalid",
                ValidationStatus.Error      => "Error",
                ValidationStatus.Skipped    => "Skipped",
                _                           => "Unknown"
            };
        }
        return string.Empty;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>Converts int queue count — 0 returns "Empty" otherwise the count.</summary>
public sealed class QueueCountConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is int i && i > 0 ? i.ToString() : "Empty";

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
