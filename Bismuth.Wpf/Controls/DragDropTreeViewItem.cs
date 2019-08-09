using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bismuth.Wpf.Controls
{
    public class DragDropTreeViewItem : MultiSelectTreeViewItem
    {
        static DragDropTreeViewItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DragDropTreeViewItem), new FrameworkPropertyMetadata(typeof(DragDropTreeViewItem)));
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new DragDropTreeViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is DragDropTreeViewItem;
        }
    }
}
