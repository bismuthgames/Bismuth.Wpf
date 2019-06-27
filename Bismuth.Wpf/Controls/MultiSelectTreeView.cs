using System.Collections;
using System.Collections.Specialized;
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

        private MultiSelectTreeViewItem PrimarySelectedContainer
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

            treeView.IsSelectionChangeActive = true;

            if (e.OldValue is INotifyCollectionChanged oldObservableList)
                oldObservableList.CollectionChanged -= treeView.CollectionChanged;

            if (e.OldValue is IList oldList)
                treeView.SetIsSelected(oldList, false);

            if (e.NewValue is IList newList)
                treeView.SetIsSelected(newList, true);

            if (e.NewValue is INotifyCollectionChanged newObservableList)
                newObservableList.CollectionChanged += treeView.CollectionChanged;

            treeView.IsSelectionChangeActive = false;
        }

        private void SetIsSelected(IList target, bool value)
        {
            foreach (var item in target)
            {
                if (ContainerFromItemRecursive(ItemContainerGenerator, item) is MultiSelectTreeViewItem treeViewItem)
                    treeViewItem.IsSelected = value;
            }
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
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

        private MultiSelectTreeViewItem GetNextSelectedItem(ItemContainerGenerator generator, MultiSelectTreeViewItem primarySelectedContainer)
        {
            for (int i = 0; i < generator.Items.Count; i++)
            {
                if (generator.ContainerFromIndex(i) is MultiSelectTreeViewItem treeViewItem)
                {
                    if (treeViewItem.IsSelected && treeViewItem != primarySelectedContainer) return treeViewItem;

                    treeViewItem = GetNextSelectedItem(treeViewItem.ItemContainerGenerator, primarySelectedContainer);
                    if (treeViewItem != null) return treeViewItem;
                }
            }

            return null;
        }

        private void SingleSelect(ItemContainerGenerator generator, MultiSelectTreeViewItem primarySelectedContainer)
        {
            for (int i = 0; i < generator.Items.Count; i++)
            {
                if (generator.ContainerFromIndex(i) is MultiSelectTreeViewItem treeViewItem)
                {
                    if (treeViewItem != primarySelectedContainer)
                        treeViewItem.IsSelected = false;

                    SingleSelect(treeViewItem.ItemContainerGenerator, primarySelectedContainer);
                }
            }
        }

        private bool MultiSelectRange(ItemContainerGenerator generator, bool isBetween, MultiSelectTreeViewItem a, MultiSelectTreeViewItem b)
        {
            for (int i = 0; i < generator.Items.Count; i++)
            {
                if (generator.ContainerFromIndex(i) is MultiSelectTreeViewItem treeViewItem)
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
            SingleSelect(ItemContainerGenerator, PrimarySelectedContainer);
        }

        internal void MultiSelect(MultiSelectTreeViewItem item)
        {
            var primarySelectedContainer = PrimarySelectedContainer;

            if (primarySelectedContainer == item)
            {
                // If this item is the primary selected item, we want to find the next item
                // (which is first item in the selection list other than the current one)
                // and make that the primary one.
                // If no items were found, we just unselect the current one.
                // NOTE: MakePrimary() will automatically unselect the current primary item.
                var nextSelectedItem = GetNextSelectedItem(ItemContainerGenerator, item);
                if (nextSelectedItem != null)
                {
                    nextSelectedItem.MakePrimary();
                }
                else
                {
                    item.IsSelected = false;
                }
            }
            else if (primarySelectedContainer == null)
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
            var primarySelectedContainer = PrimarySelectedContainer;

            IsSelectionChangeActive = true;
            MultiSelectRange(ItemContainerGenerator, false, primarySelectedContainer, item);
            IsSelectionChangeActive = false;
        }
    }
}
