using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Bismuth.Wpf.Converters
{
    public class IsLastItemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = (DependencyObject)value;
            var itemContainerGenerator = ItemsControl.ItemsControlFromItemContainer(item).ItemContainerGenerator;

            int index = itemContainerGenerator.IndexFromContainer(item);
            return index == itemContainerGenerator.Items.Count - 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
