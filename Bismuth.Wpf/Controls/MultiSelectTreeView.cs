using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Bismuth.Wpf.Extensions;
using Bismuth.Wpf.Helpers;

namespace Bismuth.Wpf.Controls
{
    public class MultiSelectTreeView : ItemsControl
    {
        private bool _suppressPrimaryItemChanged;
        private bool _suppressCollectionChanged;

        public object PrimaryItem
        {
            get { return GetValue(PrimaryItemProperty); }
            set { SetValue(PrimaryItemProperty, value); }
        }

        public static readonly DependencyProperty PrimaryItemProperty =
            DependencyProperty.Register(nameof(PrimaryItem), typeof(object), typeof(MultiSelectTreeView),
                new PropertyMetadata((d, e) => ((MultiSelectTreeView)d).SetPrimaryItem(e.NewValue)));

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
            try
            {
                _suppressCollectionChanged = true;

                if (e.OldValue is INotifyCollectionChanged oldObservableList)
                    oldObservableList.CollectionChanged -= CollectionChanged;

                UnselectAll();

                if (e.NewValue is IList newList)
                    SetIsSelected(newList, true);

                EnsurePrimaryItem();

                if (e.NewValue is INotifyCollectionChanged newObservableList)
                    newObservableList.CollectionChanged += CollectionChanged;
            }
            finally
            {
                _suppressCollectionChanged = false;
            }
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

                EnsurePrimaryItem();
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
            if (SelectedItems == null || SelectedItems.IsReadOnly || SelectedItems.IsFixedSize) return;

            if (_suppressCollectionChanged) return;

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
            if (SelectedItems == null || SelectedItems.IsReadOnly || SelectedItems.IsFixedSize) return;

            if (_suppressCollectionChanged) return;

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

            EnsurePrimaryItem();
        }

        internal void ClearSelectedItems()
        {
            try
            {
                _suppressCollectionChanged = true;

                PrimaryItem = null;

                if (SelectedItems != null && !SelectedItems.IsReadOnly && !SelectedItems.IsFixedSize)
                    SelectedItems.Clear();

                UnselectAll();
            }
            finally
            {
                _suppressCollectionChanged = false;
            }
        }

        internal MultiSelectTreeViewItem PrimaryContainer { get; private set; }
        internal MultiSelectTreeViewItem SecondaryContainer { get; private set; }

        internal void SetPrimaryItem(object item)
        {
            if (_suppressPrimaryItemChanged) return;

            try
            {
                _suppressPrimaryItemChanged = true;

                if (item == null)
                    item = SelectedItems?.Cast<object>().FirstOrDefault();

                if (PrimaryContainer != null)
                    PrimaryContainer.IsPrimary = false;

                PrimaryItem = item;
                PrimaryContainer = EnumerateContainers().FirstOrDefault(i => i.ItemForContainer == item);
                SecondaryContainer = PrimaryContainer;

                if (PrimaryContainer != null)
                    PrimaryContainer.IsPrimary = true;
            }
            finally
            {
                _suppressPrimaryItemChanged = false;
            }
        }

        internal void EnsurePrimaryItem()
        {
            if (SelectedItems != null && PrimaryItem != null &&
                SelectedItems.Contains(PrimaryItem)) return;

            PrimaryItem = SelectedItems?.Cast<object>()?.FirstOrDefault();
        }

        internal void MultiSelect(MultiSelectTreeViewItem container)
        {
            if (PrimaryContainer == null)
                container.IsPrimary = true;

            SecondaryContainer = container;

            MultiSelectRange(PrimaryContainer, SecondaryContainer);
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
                if (generator.ContainerFromIndex(i) is MultiSelectTreeViewItem container)
                {
                    yield return container;

                    if (includeChildren(container))
                        foreach (var child in EnumerateContainers(container.ItemContainerGenerator, includeChildren))
                            yield return child;
                }
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new MultiSelectTreeViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is MultiSelectTreeViewItem;
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            PreloadContainers();

            base.OnItemsSourceChanged(oldValue, newValue);
        }

        private void PreloadContainers()
        {
            foreach (var container in EnumerateContainers())
            {
                container.ApplyTemplate();
                if (container.Template.FindName("ItemsHost", container) is ItemsPresenter itemsPresenter)
                    itemsPresenter.ApplyTemplate();

                container.GetItemsHost().EnsureGenerator();
            }
        }
    }
}
