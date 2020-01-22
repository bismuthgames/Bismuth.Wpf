using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Bismuth.Wpf.Controls
{
    public class MultiSelectTreeViewItem : HeaderedItemsControl
    {
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(MultiSelectTreeViewItem),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => ((MultiSelectTreeViewItem)d).OnIsSelectedChanged(e)));

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register(nameof(IsExpanded), typeof(bool), typeof(MultiSelectTreeViewItem),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => ((MultiSelectTreeViewItem)d).OnIsExpandedChanged(e)));

        private void OnIsSelectedChanged(DependencyPropertyChangedEventArgs e)
        {

        }

        private void OnIsExpandedChanged(DependencyPropertyChangedEventArgs e)
        {

        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new MultiSelectTreeViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is MultiSelectTreeViewItem;
        }

        internal object ItemForContainer { get { return ParentItemsControl.ItemContainerGenerator.ItemFromContainer(this); } }

        internal ItemsControl ParentItemsControl
        {
            get { return ItemsControlFromItemContainer(this); }
        }

        internal MultiSelectTreeView ParentTreeView
        {
            get
            {
                var itemsControl = ParentItemsControl;
                while (itemsControl != null)
                {
                    if (itemsControl is MultiSelectTreeView treeView) return treeView;

                    itemsControl = ItemsControlFromItemContainer(itemsControl);
                }

                return null;
            }
        }

        private MultiSelectTreeViewItem GetPrevious()
        {
            return ParentTreeView
                .EnumerateTreeViewItems(i => i.IsExpanded)
                .TakeWhile(i => i != this)
                .LastOrDefault();
        }

        private MultiSelectTreeViewItem GetNext()
        {
            return ParentTreeView
                .EnumerateTreeViewItems(i => i.IsExpanded)
                .SkipWhile(i => i != this)
                .Skip(1)
                .FirstOrDefault();
        }
    }
}
