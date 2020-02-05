using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Bismuth.Wpf.Converters
{
    public class DoubleToThicknessConverter : IValueConverter
    {
        public Thickness Scale { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double scale = System.Convert.ToDouble(value);
            return new Thickness
            (
                scale * Scale.Left,
                scale * Scale.Top,
                scale * Scale.Right,
                scale * Scale.Bottom
            );
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
