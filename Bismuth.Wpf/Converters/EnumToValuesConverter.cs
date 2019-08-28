﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace Bismuth.Wpf.Converters
{
    public class EnumToValuesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Type type && type.IsEnum)
                return Enum.GetValues(type);

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
