using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Bismuth.Wpf.Converters
{
    public class IsPreviousItemSelectedConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var item = (DependencyObject)values[0];
            int index = ItemsControl.ItemsControlFromItemContainer(item).ItemContainerGenerator.IndexFromContainer(item);
            int selectedIndex = (int)values[1];
            return selectedIndex == index - 1;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
