using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Bismuth.Wpf.Helpers;

namespace Bismuth.Wpf.Controls
{
    public class MultiSelectTreeView : ItemsControl
    {
        protected override DependencyObject GetContainerForItemOverride() => new MultiSelectTreeViewItem();
        protected override bool IsItemItsOwnContainerOverride(object item) => item is MultiSelectTreeViewItem;

        internal MultiSelectTreeViewItem PrimarySelectedContainer { get; private set; }

        private bool _suppressCollectionChanged;
        private bool _suppressPrimarySelectedItemChanged;

        public object PrimarySelectedItem
        {
            get { return GetValue(PrimarySelectedItemProperty); }
            set { SetValue(PrimarySelectedItemProperty, value); }
        }

        public static readonly DependencyProperty PrimarySelectedItemProperty =
            DependencyProperty.Register(nameof(PrimarySelectedItem), typeof(object), typeof(MultiSelectTreeView), new PropertyMetadata(null,
                (d, e) => ((MultiSelectTreeView)d).OnPrimarySelectedItemChanged(e)));

        private void OnPrimarySelectedItemChanged(DependencyPropertyChangedEventArgs e)
        {
            if (_suppressPrimarySelectedItemChanged) return;

            try
            {
                _suppressPrimarySelectedItemChanged = true;

                if (PrimarySelectedContainer != null)
                    PrimarySelectedContainer.IsPrimarySelected = false;

                PrimarySelectedContainer = EnumerateContainers().FirstOrDefault(i => i.ItemForContainer == e.NewValue);

                if (PrimarySelectedContainer != null)
                    PrimarySelectedContainer.IsPrimarySelected = true;
            }
            finally
            {
                _suppressPrimarySelectedItemChanged = false;
            }
        }

        public IList SelectedItems
        {
            get { return (IList)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register(nameof(SelectedItems), typeof(IList), typeof(MultiSelectTreeView),
                new PropertyMetadata(DefaultValueFactory.CreateObservableCollection<object>(), (d, e) => ((MultiSelectTreeView)d).OnSelectedItemsChanged(e)));

        private void OnSelectedItemsChanged(DependencyPropertyChangedEventArgs e)
        {
            _suppressCollectionChanged = true;

            if (e.OldValue is INotifyCollectionChanged oldObservableList)
                oldObservableList.CollectionChanged -= CollectionChanged;

            UnselectAll();

            if (e.NewValue is IList newList)
                SetIsSelected(newList, true);

            EnsurePrimarySelectedItem();

            if (e.NewValue is INotifyCollectionChanged newObservableList)
                newObservableList.CollectionChanged += CollectionChanged;

            _suppressCollectionChanged = false;
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_suppressCollectionChanged) return;

            try
            {
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

                EnsurePrimarySelectedItem();
            }
            finally
            {
                _suppressCollectionChanged = false;
            }
        }

        private void SetIsSelected(IList items, bool value)
        {
            foreach (var i in EnumerateContainers())
                if (items.Contains(i.ItemForContainer))
                    i.IsSelected = value;
        }

        internal void EnsurePrimarySelectedItem()
        {
            if (SelectedItems != null && PrimarySelectedItem != null &&
                SelectedItems.Contains(PrimarySelectedItem)) return;

            PrimarySelectedItem = SelectedItems?.Cast<object>()?.FirstOrDefault();

            //if (PrimarySelectedContainer != null &&
            //    PrimarySelectedContainer.ParentTreeView == this &&
            //    PrimarySelectedContainer.IsSelected)
            //    return;
            //PrimarySelectedItem = EnumerateContainers().FirstOrDefault(i => i.IsSelected)?.ItemForContainer;
        }

        internal void SelectItem(object item)
        {
            if (_suppressCollectionChanged) return;
            if (SelectedItems == null || SelectedItems.IsReadOnly || SelectedItems.IsFixedSize) return;

            try
            {
                _suppressCollectionChanged = true;

                SelectedItems.Add(item);
            }
            finally
            {
                _suppressCollectionChanged = false;
            }
        }

        internal void UnselectItem(object item)
        {
            if (_suppressCollectionChanged) return;
            if (SelectedItems == null || SelectedItems.IsReadOnly || SelectedItems.IsFixedSize) return;

            try
            {
                _suppressCollectionChanged = true;

                SelectedItems.Remove(item);
            }
            finally
            {
                _suppressCollectionChanged = false;
            }
        }

        internal void UnselectAll()
        {
            foreach (var i in EnumerateContainers())
                i.IsSelected = false;
        }

        //internal void UnselectAllExceptPrimary()
        //{
        //    var primarySelectedContainer = PrimarySelectedContainer;
        //    foreach (var i in EnumerateContainers().Where(i => i != primarySelectedContainer))
        //        i.IsSelected = false;
        //}

        internal void RefreshSelectedItems()
        {
            if (SelectedItems == null || SelectedItems.IsReadOnly || SelectedItems.IsFixedSize || SelectedItems.Count == 0) return;

            var oldSelectedItems = SelectedItems
                .Cast<object>()
                .ToArray();

            var newSelectedItems = EnumerateContainers(i => i.IsExpanded)
                .Where(i => i.IsSelected)
                .Select(i => i.ItemForContainer)
                .ToArray();

            foreach (object item in oldSelectedItems.Except(newSelectedItems))
                UnselectItem(item);

            foreach (object item in newSelectedItems.Except(oldSelectedItems))
                SelectItem(item);

            EnsurePrimarySelectedItem();
        }

        internal void ClearSelectedItems()
        {
            _suppressCollectionChanged = true;

            PrimarySelectedItem = null;

            if (SelectedItems != null && !SelectedItems.IsReadOnly && !SelectedItems.IsFixedSize)
                SelectedItems.Clear();

            UnselectAll();

            _suppressCollectionChanged = false;
        }

        //private void SingleSelect(MultiSelectTreeViewItem container)
        //{
        //    _suppressCollectionChanged = true;

        //    if (SelectedItems != null && !SelectedItems.IsReadOnly && !SelectedItems.IsFixedSize)
        //        SelectedItems.Clear();

        //    UnselectAll();

        //    _suppressCollectionChanged = false;
        //}

        internal void MultiSelect(MultiSelectTreeViewItem container)
        {
            if (PrimarySelectedContainer == null)
                container.IsPrimarySelected = true;

            MultiSelectRange(PrimarySelectedContainer, container);
        }

        //internal void MultiSelect(MultiSelectTreeViewItem container)
        //{
        //    var primarySelectedContainer = _primarySelectedContainer;

        //    if (primarySelectedContainer == container)
        //    {
        //        // If this item is the primary selected item, we want to find the next item
        //        // (which is first item in the selection list other than the current one)
        //        // and make that the primary one.
        //        // If no items were found, we just unselect the current one.
        //        // NOTE: MakePrimary() will automatically unselect the current primary item.
        //        var nextSelectedContainer = EnumerateTreeViewItems().FirstOrDefault(i => i.IsSelected && i != container);
        //        if (nextSelectedContainer != null)
        //        {
        //            nextSelectedContainer.MakePrimary();
        //        }
        //        else
        //        {
        //            container.IsSelected = false;
        //        }
        //    }
        //    else if (primarySelectedContainer == null)
        //    {
        //        container.IsSelected = !container.IsSelected;
        //    }
        //    else if (container.IsSelected)
        //    {
        //        container.IsSelected = false;
        //    }
        //    else
        //    {
        //        container.IsSelected = true;
        //    }
        //}

        //internal void MultiSelectRange(OldMultiSelectTreeViewItem container)
        //{
        //    SecondarySelectedContainer = container;

        //    if (PrimarySelectedContainer == null) SelectFirst();
        //    if (PrimarySelectedContainer == null) return;

        //    IsSelectionChangeActive = true;
        //    MultiSelectRange(PrimarySelectedContainer, SecondarySelectedContainer);
        //    IsSelectionChangeActive = false;
        //}

        private void MultiSelectRange(MultiSelectTreeViewItem a, MultiSelectTreeViewItem b)
        {
            bool isBetween = false;

            foreach (var i in EnumerateContainers(i => i.IsExpanded))
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

        //private void SelectFirst()
        //{
        //    if (ItemContainerGenerator.Items.Count > 0 &&
        //        ItemContainerGenerator.ContainerFromIndex(0) is MultiSelectTreeViewItem treeViewItem)
        //        treeViewItem.IsSelected = true;
        //}

        internal IEnumerable<MultiSelectTreeViewItem> EnumerateContainers()
        {
            return EnumerateContainers(ItemContainerGenerator, _ => true);
        }

        internal IEnumerable<MultiSelectTreeViewItem> EnumerateContainers(Func<MultiSelectTreeViewItem, bool> includeChildren)
        {
            return EnumerateContainers(ItemContainerGenerator, includeChildren);
        }

        private IEnumerable<MultiSelectTreeViewItem> EnumerateContainers(ItemContainerGenerator generator, Func<MultiSelectTreeViewItem, bool> includeChildren)
        {
            for (int i = 0; i < generator.Items.Count; i++)
            {
                if (generator.ContainerFromIndex(i) is MultiSelectTreeViewItem treeViewItem)
                {
                    yield return treeViewItem;

                    if (includeChildren(treeViewItem))
                        foreach (var child in EnumerateContainers(treeViewItem.ItemContainerGenerator, includeChildren))
                            yield return child;
                }
            }
        }
    }
}
