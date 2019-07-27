using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Bismuth.Wpf.Helpers;

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
            DependencyProperty.Register(nameof(SelectedItems), typeof(IList), typeof(MultiSelectTreeView),
                new PropertyMetadata(DefaultValueFactory.CreateObservableCollection<object>(), OnSelectedItemsChanged));

        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var treeView = (MultiSelectTreeView)d;

            treeView._suppressCollectionChanged = true;
            treeView.IsSelectionChangeActive = true;

            if (e.OldValue is INotifyCollectionChanged oldObservableList)
                oldObservableList.CollectionChanged -= treeView.CollectionChanged;

            if (e.OldValue is IList oldList)
                treeView.SetIsSelected(oldList, false);

            if (e.NewValue is IList newList)
                treeView.SetIsSelected(newList, true);

            if (e.NewValue is INotifyCollectionChanged newObservableList)
                newObservableList.CollectionChanged += treeView.CollectionChanged;

            treeView._suppressCollectionChanged = false;
            treeView.IsSelectionChangeActive = false;
        }

        private bool _suppressCollectionChanged;

        private void SetIsSelected(IList target, bool value)
        {
            foreach (var i in EnumerateTreeViewItems())
                if (target.Contains(i.ItemForContainer))
                    i.IsSelected = value;
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_suppressCollectionChanged) return;
            _suppressCollectionChanged = true;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:

                    if (e.OldItems != null) SetIsSelected(e.OldItems, false);
                    if (e.NewItems != null) SetIsSelected(e.NewItems, true);

                    break;

                case NotifyCollectionChangedAction.Reset:

                    UnselectAll();
                    if (SelectedItems is IList list)
                        SetIsSelected(list, true);

                    break;
            }

            _suppressCollectionChanged = false;
        }

        internal void AddToSelected(object item)
        {
            if (SelectedItems == null || SelectedItems.IsReadOnly) return;

            if (_suppressCollectionChanged) return;
            _suppressCollectionChanged = true;

            SelectedItems.Add(item);

            _suppressCollectionChanged = false;
        }

        internal void RemoveFromSelected(object item)
        {
            if (SelectedItems == null || SelectedItems.IsReadOnly) return;

            if (_suppressCollectionChanged) return;
            _suppressCollectionChanged = true;

            SelectedItems.Remove(item);

            _suppressCollectionChanged = false;
        }

        internal void UnselectAll()
        {
            foreach (var i in EnumerateTreeViewItems())
                i.IsSelected = false;
        }

        internal void UnselectAllExceptPrimary()
        {
            var primarySelectedContainer = PrimarySelectedContainer;
            foreach (var i in EnumerateTreeViewItems().Where(i => i != primarySelectedContainer))
                i.IsSelected = false;
        }

        internal MultiSelectTreeViewItem SecondarySelectedContainer { get; private set; }

        internal void MultiSelect(MultiSelectTreeViewItem container)
        {
            var primarySelectedContainer = PrimarySelectedContainer;

            if (primarySelectedContainer == container)
            {
                // If this item is the primary selected item, we want to find the next item
                // (which is first item in the selection list other than the current one)
                // and make that the primary one.
                // If no items were found, we just unselect the current one.
                // NOTE: MakePrimary() will automatically unselect the current primary item.
                var nextSelectedContainer = EnumerateTreeViewItems().FirstOrDefault(i => i.IsSelected && i != container);
                if (nextSelectedContainer != null)
                {
                    nextSelectedContainer.MakePrimary();
                }
                else
                {
                    container.IsSelected = false;
                }
            }
            else if (primarySelectedContainer == null)
            {
                container.IsSelected = !container.IsSelected;
            }
            else if (container.IsSelected)
            {
                container.IsSelected = false;
            }
            else
            {
                IsSelectionChangeActive = true;
                container.IsSelected = true;
                IsSelectionChangeActive = false;
            }
        }

        internal void MultiSelectRange(MultiSelectTreeViewItem container)
        {
            SecondarySelectedContainer = container;

            if (PrimarySelectedContainer == null) SelectFirst();
            if (PrimarySelectedContainer == null) return;

            IsSelectionChangeActive = true;
            MultiSelectRange(PrimarySelectedContainer, SecondarySelectedContainer);
            IsSelectionChangeActive = false;
        }

        private void MultiSelectRange(MultiSelectTreeViewItem a, MultiSelectTreeViewItem b)
        {
            bool isBetween = false;

            foreach (var i in EnumerateTreeViewItems(i => i.IsExpanded))
            {
                if (i == a || i == b)
                {
                    i.IsSelected = true;
                    if (a != b) isBetween = !isBetween;
                }
                else
                {
                    i.IsSelected = isBetween;
                }

                if (!i.IsExpanded)
                    i.UnselectRecursive();
            }
        }

        private void SelectFirst()
        {
            if (ItemContainerGenerator.Items.Count > 0 &&
                ItemContainerGenerator.ContainerFromIndex(0) is MultiSelectTreeViewItem treeViewItem)
                treeViewItem.IsSelected = true;
        }

        protected override void OnSelectedItemChanged(RoutedPropertyChangedEventArgs<object> e)
        {
            SecondarySelectedContainer = PrimarySelectedContainer;
        }

        internal IEnumerable<MultiSelectTreeViewItem> EnumerateTreeViewItems()
        {
            return EnumerateTreeViewItems(ItemContainerGenerator, _ => true);
        }

        internal IEnumerable<MultiSelectTreeViewItem> EnumerateTreeViewItems(Func<MultiSelectTreeViewItem, bool> includeChildren)
        {
            return EnumerateTreeViewItems(ItemContainerGenerator, includeChildren);
        }

        private IEnumerable<MultiSelectTreeViewItem> EnumerateTreeViewItems(ItemContainerGenerator generator, Func<MultiSelectTreeViewItem, bool> includeChildren)
        {
            for (int i = 0; i < generator.Items.Count; i++)
            {
                if (generator.ContainerFromIndex(i) is MultiSelectTreeViewItem treeViewItem)
                {
                    yield return treeViewItem;

                    if (includeChildren(treeViewItem))
                        foreach (var child in EnumerateTreeViewItems(treeViewItem.ItemContainerGenerator, includeChildren))
                            yield return child;
                }
            }
        }
    }
}
