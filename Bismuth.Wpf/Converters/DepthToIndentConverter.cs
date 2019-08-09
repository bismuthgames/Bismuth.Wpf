using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Bismuth.Wpf.Extensions;

namespace Bismuth.Wpf.Converters
{
    public class DepthToIndentConverter : IValueConverter
    {
        public double IndentLength { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TreeViewItem item)
                return new Thickness(GetDepth(item) * IndentLength, 0, 0, 0);

            return new Thickness(0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        private int GetDepth(TreeViewItem item)
        {
            var parent = item.FindVisualParent<TreeViewItem>();
            if (parent != null) return GetDepth(parent) + 1;

            return 0;
        }
    }
}
