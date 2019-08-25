using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Bismuth.Wpf.Controls
{
    public class PropertyGroupStyleSelector : StyleSelector
    {
        public Style DefaultStyle { get; set; }
        public Style CategoryStyle { get; set; }
        public Style AdvancedStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            var group = (CollectionViewGroup)item;
            if (group.Name is bool isAdvanced) return isAdvanced ? AdvancedStyle : DefaultStyle;

            return CategoryStyle;
        }
    }
}
