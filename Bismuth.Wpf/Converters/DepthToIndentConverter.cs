using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Bismuth.Wpf.Controls;
using Bismuth.Wpf.Extensions;

namespace Bismuth.Wpf.Converters
{
    public class DepthToIndentConverter : IValueConverter
    {
        public double IndentLength { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MultiSelectTreeViewItem item)
                return new Thickness(GetDepth(item) * IndentLength, 0, 0, 0);

            return new Thickness(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        private int GetDepth(MultiSelectTreeViewItem item)
        {
            var parent = item.FindVisualParent<MultiSelectTreeViewItem>();
            if (parent != null) return GetDepth(parent) + 1;

            return 0;
        }
    }
}
