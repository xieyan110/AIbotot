using System;
using System.Windows;
using System.Globalization;
using System.Windows.Data;
using Nodify;

namespace Aibot
{
    public class ConnectorOffsetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double offset = System.Convert.ToDouble(parameter);
            if (value is Size s)
            {
                return new Size((s.Width + offset) / 2, (s.Height + offset) / 2);
            }

            return new Size(offset / 2, offset / 2);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double offset = System.Convert.ToDouble(parameter);
            if (value is Size s)
            {
                return new Size((s.Width + offset) / 2, (s.Height + offset) / 2);
            }

            return new Size(offset / 2, offset / 2);
        }
    }

    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Collapsed : Visibility.Visible;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
