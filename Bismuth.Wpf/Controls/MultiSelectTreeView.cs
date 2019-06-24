using System.Collections;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Bismuth.Wpf.Controls
{
    public class MultiSelectTreeView : TreeView
    {
        private static readonly PropertyInfo _isSelectionChangeActiveProperty =
            typeof(TreeView).GetProperty("IsSelectionChangeActive", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly PropertyInfo _selectedContainerProperty =
            typeof(TreeView).GetProperty("SelectedContainer", BindingFlags.Instance | BindingFlags.NonPublic);

        private bool IsSelectionChangeActive
        {
            get { return (bool)_isSelectionChangeActiveProperty.GetValue(this); }
            set { _isSelectionChangeActiveProperty.SetValue(this, value); }
        }

        private MultiSelectTreeViewItem SelectedContainer
        {
            get { return _selectedContainerProperty.GetValue(this) as MultiSelectTreeViewItem; }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new MultiSelectTreeViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is MultiSelectTreeViewItem;
        }

        public IList SelectedItems
        {
            get { return (IList)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register(nameof(SelectedItems), typeof(IList), typeof(MultiSelectTreeView), new PropertyMetadata(OnSelectedItemsChanged));

        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var treeView = (MultiSelectTreeView)d;
        }

        private DependencyObject ContainerFromItemRecursive(ItemContainerGenerator generator, object item)
        {
            for (int i = 0; i < generator.Items.Count; i++)
            {
                var obj = generator.Items[i];
                var container = generator.ContainerFromIndex(i);
                if (obj == item) return container;

                var itemsControl = container as ItemsControl;
                if (itemsControl == null) continue;

                container = ContainerFromItemRecursive(itemsControl.ItemContainerGenerator, item);
                if (container != null) return container;
            }

            return null;
        }

        private MultiSelectTreeViewItem GetNextSelectedItem(ItemContainerGenerator generator, MultiSelectTreeViewItem selectedContainer)
        {
            for (int i = 0; i < generator.Items.Count; i++)
            {
                var treeViewItem = generator.ContainerFromIndex(i) as MultiSelectTreeViewItem;
                if (treeViewItem != null)
                {
                    if (treeViewItem.IsSelected && treeViewItem != selectedContainer) return treeViewItem;

                    treeViewItem = GetNextSelectedItem(treeViewItem.ItemContainerGenerator, selectedContainer);
                    if (treeViewItem != null) return treeViewItem;
                }
            }

            return null;
        }

        private void SingleSelect(ItemContainerGenerator generator, MultiSelectTreeViewItem selectedContainer)
        {
            for (int i = 0; i < generator.Items.Count; i++)
            {
                var treeViewItem = generator.ContainerFromIndex(i) as MultiSelectTreeViewItem;
                if (treeViewItem != null)
                {
                    if (treeViewItem != selectedContainer)
                        treeViewItem.IsSelected = false;

                    SingleSelect(treeViewItem.ItemContainerGenerator, selectedContainer);
                }
            }
        }

        private bool MultiSelectRange(ItemContainerGenerator generator, bool isBetween, MultiSelectTreeViewItem a, MultiSelectTreeViewItem b)
        {
            for (int i = 0; i < generator.Items.Count; i++)
            {
                var treeViewItem = generator.ContainerFromIndex(i) as MultiSelectTreeViewItem;
                if (treeViewItem != null)
                {
                    if (treeViewItem == a || treeViewItem == b)
                    {
                        treeViewItem.IsSelected = true;
                        isBetween = !isBetween;
                    }
                    else
                    {
                        treeViewItem.IsSelected = isBetween;
                    }

                    if (treeViewItem.IsExpanded)
                        isBetween = MultiSelectRange(treeViewItem.ItemContainerGenerator, isBetween, a, b);
                    else
                        treeViewItem.UnselectRecursive();
                }
            }

            return isBetween;
        }

        internal void AddToSelected(MultiSelectTreeViewItem item)
        {

        }

        internal void RemoveFromSelected(MultiSelectTreeViewItem item)
        {

        }

        internal void UnselectAllExceptPrimary()
        {
            SingleSelect(ItemContainerGenerator, SelectedContainer);
        }

        internal void MultiSelect(MultiSelectTreeViewItem item)
        {
            var selectedContainer = SelectedContainer;

            if (selectedContainer == item)
            {
                var nextSelectedItem = GetNextSelectedItem(ItemContainerGenerator, item);
                if (nextSelectedItem != null)
                {
                    // If more than one items is selected, we want to make the
                    // next selected item primary one when the current is unselected.
                    nextSelectedItem.Select(true);
                }
                else
                {
                    item.IsSelected = false;
                }
            }
            else if (selectedContainer == null)
            {
                item.IsSelected = !item.IsSelected;
            }
            else if (item.IsSelected)
            {
                item.IsSelected = false;
            }
            else
            {
                IsSelectionChangeActive = true;
                item.IsSelected = true;
                IsSelectionChangeActive = false;
            }
        }

        internal void MultiSelectRange(MultiSelectTreeViewItem item)
        {
            var selectedContainer = SelectedContainer;

            IsSelectionChangeActive = true;
            MultiSelectRange(ItemContainerGenerator, false, selectedContainer, item);
            IsSelectionChangeActive = false;
        }
    }
}
