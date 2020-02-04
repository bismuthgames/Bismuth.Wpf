using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
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
        internal MultiSelectTreeViewItem SecondarySelectedContainer { get; private set; }

        private bool _suppressCollectionChanged;
        private bool _suppressPrimarySelectedItemChanged;

        public object PrimarySelectedItem
        {
            get { return GetValue(PrimarySelectedItemProperty); }
            set { SetValue(PrimarySelectedItemProperty, value); }
        }

        public static readonly DependencyProperty PrimarySelectedItemProperty =
            DependencyProperty.Register(nameof(PrimarySelectedItem), typeof(object), typeof(MultiSelectTreeView), new PropertyMetadata(null,
                (d, e) => ((MultiSelectTreeView)d).SetPrimarySelectedItem(e.NewValue)));
                //(d, e) => ((MultiSelectTreeView)d).CoercePrimarySelectedItem(e)));

        internal void SetPrimarySelectedItem(object item)
        {
            if (_suppressPrimarySelectedItemChanged) return;

            try
            {
                _suppressPrimarySelectedItemChanged = true;

                if (item == null)
                    item = SelectedItems?.Cast<object>().FirstOrDefault();

                if (PrimarySelectedContainer != null)
                    PrimarySelectedContainer.IsPrimarySelected = false;

                PrimarySelectedItem = item;
                PrimarySelectedContainer = EnumerateContainers().FirstOrDefault(i => i.ItemForContainer == item);
                SecondarySelectedContainer = PrimarySelectedContainer;

                if (PrimarySelectedContainer != null)
                    PrimarySelectedContainer.IsPrimarySelected = true;
            }
            finally
            {
                _suppressPrimarySelectedItemChanged = false;
            }
        }

        private object CoercePrimarySelectedItem(object baseValue)
        {
            return baseValue ?? SelectedItems?.Cast<object>()?.FirstOrDefault();
        }

        internal void EnsurePrimarySelectedItem()
        {
            if (SelectedItems != null && PrimarySelectedItem != null &&
                SelectedItems.Contains(PrimarySelectedItem)) return;

            PrimarySelectedItem = SelectedItems?.Cast<object>()?.FirstOrDefault();
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

        internal void MultiSelect(MultiSelectTreeViewItem container)
        {
            if (PrimarySelectedContainer == null)
                container.IsPrimarySelected = true;

            SecondarySelectedContainer = container;

            MultiSelectRange(PrimarySelectedContainer, SecondarySelectedContainer);
        }

        internal void MultiSelectRange(MultiSelectTreeViewItem container)
        {
            SecondarySelectedContainer = container;

            if (PrimarySelectedContainer == null)
                container.IsPrimarySelected = true;

            //if (PrimarySelectedContainer == null) SelectFirst();
            //if (PrimarySelectedContainer == null) return;

            MultiSelectRange(PrimarySelectedContainer, SecondarySelectedContainer);
        }

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
