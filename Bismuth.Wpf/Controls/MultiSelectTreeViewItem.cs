using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Bismuth.Wpf.Extensions;

namespace Bismuth.Wpf.Controls
{
    public class MultiSelectTreeViewItem : HeaderedItemsControl
    {
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public object IsPrimarySelected
        {
            get { return GetValue(IsPrimarySelectedProperty); }
            set { SetValue(IsPrimarySelectedProperty, value); }
        }

        public static readonly DependencyProperty IsPrimarySelectedProperty = DependencyProperty.Register
        (
            nameof(IsPrimarySelected),
            typeof(object),
            typeof(MultiSelectTreeViewItem),
            new FrameworkPropertyMetadata
            (
                defaultValue: false,
                flags: FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                propertyChangedCallback: (d, e) => ((MultiSelectTreeViewItem)d).OnIsPrimarySelectedChanged(e)
            )
        );


        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(MultiSelectTreeViewItem),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    (d, e) => ((MultiSelectTreeViewItem)d).OnIsSelectedChanged(e)));

        public bool IsExpanded
        {
            get { return (bool)GetValue(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register(nameof(IsExpanded), typeof(bool), typeof(MultiSelectTreeViewItem),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    (d, e) => ((MultiSelectTreeViewItem)d).OnIsExpandedChanged(e)));

        private void OnIsPrimarySelectedChanged(DependencyPropertyChangedEventArgs e)
        {
            var parentTreeView = ParentTreeView;
            if (parentTreeView == null) return;

            if ((bool)e.NewValue)
            {
                parentTreeView.PrimarySelectedItem = ItemForContainer;
                if (!IsSelected) IsSelected = true;
            }
            else
            {
                parentTreeView.EnsurePrimarySelectedItem();
                //parentTreeView.PrimarySelectedItem = null;
            }
        }

        private void OnIsSelectedChanged(DependencyPropertyChangedEventArgs e)
        {
            var parentTreeView = ParentTreeView;
            if (parentTreeView == null) return;

            if ((bool)e.NewValue)
            {
                parentTreeView.SelectItem(ItemForContainer);
            }
            else
            {
                parentTreeView.UnselectItem(ItemForContainer);
            }

            parentTreeView.EnsurePrimarySelectedItem();
        }

        private void OnIsExpandedChanged(DependencyPropertyChangedEventArgs e)
        {
            if (!(bool)e.NewValue) UnselectRecursive();
        }

        internal void UnselectRecursive()
        {
            for (int i = 0; i < ItemContainerGenerator.Items.Count; i++)
            {
                if (ItemContainerGenerator.ContainerFromIndex(i) is MultiSelectTreeViewItem treeViewItem)
                {
                    treeViewItem.IsSelected = false;
                    treeViewItem.UnselectRecursive();
                }
            }
        }

        private static bool IsControlKeyDown => (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
        private static bool IsShiftKeyDown => (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (IsShiftKeyDown)
            {
                e.Handled = true;

                //if (Keyboard.IsKeyDown(Key.Up))
                //{
                //    var previous = ParentTreeView.SecondarySelectedContainer.GetPrevious();
                //    if (previous != null) ParentTreeView.MultiSelectRange(previous);
                //}
                //else if (Keyboard.IsKeyDown(Key.Down))
                //{
                //    var next = ParentTreeView.SecondarySelectedContainer.GetNext();
                //    if (next != null) ParentTreeView.MultiSelectRange(next);
                //}
                //else if (Keyboard.IsKeyDown(Key.PageUp) || Keyboard.IsKeyDown(Key.Home))
                //{
                //    ParentTreeView.MultiSelectRange(ParentTreeView.EnumerateTreeViewItems(i => i.IsExpanded).First());
                //}
                //else if (Keyboard.IsKeyDown(Key.PageDown) || Keyboard.IsKeyDown(Key.End))
                //{
                //    ParentTreeView.MultiSelectRange(ParentTreeView.EnumerateTreeViewItems(i => i.IsExpanded).Last());
                //}
            }
            else if (Keyboard.IsKeyDown(Key.Up) ||
                     Keyboard.IsKeyDown(Key.Down) ||
                     Keyboard.IsKeyDown(Key.Left) ||
                     Keyboard.IsKeyDown(Key.Right))
            {
                ParentTreeView.ClearSelectedItems();
                IsPrimarySelected = true;
            }

            base.OnKeyDown(e);
        }

        private bool _suppressMouseLeftButtonUp;

        private bool IsThis(object originalSource)
        {
            return originalSource is DependencyObject f && f.FindVisualParent<MultiSelectTreeViewItem>() == this;
        }

        private void SafeClick(MouseButtonEventArgs e)
        {
            Console.WriteLine("Safe click!");

            if (IsControlKeyDown)
            {
                IsSelected = !IsSelected;
            }
            else if (IsShiftKeyDown)
            {
                ParentTreeView.MultiSelect(this);
            }
            else
            {
                ParentTreeView.ClearSelectedItems();
                IsPrimarySelected = true;
            }

            e.Handled = true;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (!_suppressMouseLeftButtonUp && !e.Handled && IsEnabled && IsSelected && IsThis(e.OriginalSource))
            {
                SafeClick(e);
            }

            _suppressMouseLeftButtonUp = false;

            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (!e.Handled && IsEnabled && IsThis(e.OriginalSource))
            {
                Focus();

                if (!IsSelected)
                {
                    _suppressMouseLeftButtonUp = true;
                    SafeClick(e);
                }

                if (e.ClickCount % 2 == 0)
                {
                    IsExpanded = !IsExpanded;
                }

                e.Handled = true;
            }

            base.OnMouseLeftButtonDown(e);
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
                .EnumerateContainers(i => i.IsExpanded)
                .TakeWhile(i => i != this)
                .LastOrDefault();
        }

        private MultiSelectTreeViewItem GetNext()
        {
            return ParentTreeView
                .EnumerateContainers(i => i.IsExpanded)
                .SkipWhile(i => i != this)
                .Skip(1)
                .FirstOrDefault();
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            Console.WriteLine("Items changed");
            if (e.Action == NotifyCollectionChangedAction.Remove ||
                e.Action == NotifyCollectionChangedAction.Reset)
                ParentTreeView?.RefreshSelectedItems();

            base.OnItemsChanged(e);
        }
    }
}
