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
        public bool IsPrimary
        {
            get { return (bool)GetValue(IsPrimaryProperty); }
            set { SetValue(IsPrimaryProperty, value); }
        }

        public static readonly DependencyProperty IsPrimaryProperty =
            DependencyProperty.Register(nameof(IsPrimary), typeof(bool), typeof(MultiSelectTreeViewItem),
                new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => ((MultiSelectTreeViewItem)d).OnIsPrimaryChanged(e)));

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

        private void OnIsPrimaryChanged(DependencyPropertyChangedEventArgs e)
        {
            var parentTreeView = ParentTreeView;
            if (parentTreeView == null) return;

            if ((bool)e.NewValue)
            {
                parentTreeView.SetPrimaryItem(ItemForContainer);
                if (!IsSelected) IsSelected = true;
            }
            else
            {
                parentTreeView.SetPrimaryItem(null);
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

            parentTreeView.EnsurePrimaryItem();
        }

        private void OnIsExpandedChanged(DependencyPropertyChangedEventArgs e)
        {
            if (!(bool)e.NewValue) UnselectRecursive();
        }

        internal void UnselectRecursive()
        {
            for (int i = 0; i < ItemContainerGenerator.Items.Count; i++)
            {
                if (ItemContainerGenerator.ContainerFromIndex(i) is MultiSelectTreeViewItem container)
                {
                    container.IsSelected = false;
                    container.UnselectRecursive();
                }
            }
        }

        internal object ItemForContainer
        {
            get { return ParentItemsControl.ItemContainerGenerator.ItemFromContainer(this); }
        }

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

        public int TreeDepth
        {
            get
            {
                int treeDepth = 0;
                var itemsControl = ParentItemsControl;
                while (itemsControl != null)
                {
                    if (itemsControl is MultiSelectTreeView) return treeDepth;

                    treeDepth++;
                    itemsControl = ItemsControlFromItemContainer(itemsControl);
                }

                return -1;
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
            if (e.Action == NotifyCollectionChangedAction.Remove ||
                e.Action == NotifyCollectionChangedAction.Reset)
                ParentTreeView?.RefreshSelectedItems();

            base.OnItemsChanged(e);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new MultiSelectTreeViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is MultiSelectTreeViewItem;
        }

        private static bool IsControlKeyDown => (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
        private static bool IsShiftKeyDown => (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;

        private bool IsThis(object originalSource)
        {
            return originalSource is DependencyObject d && d.FindVisualParent<MultiSelectTreeViewItem>() == this;
        }

        private void SingleSelect()
        {
            ParentTreeView.ClearSelectedItems();
            IsPrimary = true;
            Focus();
        }

        private void ToggleSelect()
        {
            IsSelected = !IsSelected;
            Focus();
        }

        private void MultiSelect()
        {
            ParentTreeView.MultiSelect(this);
            Focus();
        }

        private void MouseSelect()
        {
            if (IsShiftKeyDown)
                MultiSelect();
            else if (IsControlKeyDown)
                ToggleSelect();
            else
                SingleSelect();
        }

        private void KeyboardSelect()
        {
            if (IsShiftKeyDown)
                MultiSelect();
            else if (IsControlKeyDown)
                Focus();
            else
                SingleSelect();
        }

        private bool _suppressMouseLeftButtonUp;

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (!e.Handled && IsEnabled && IsThis(e.OriginalSource))
            {
                if (!IsSelected)
                {
                    _suppressMouseLeftButtonUp = true;
                    MouseSelect();
                }

                if (e.ClickCount % 2 == 0)
                {
                    IsExpanded = !IsExpanded;
                }

                e.Handled = true;
            }

            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (!_suppressMouseLeftButtonUp && !e.Handled && IsEnabled && IsSelected && IsThis(e.OriginalSource))
            {
                MouseSelect();
                e.Handled = true;
            }

            _suppressMouseLeftButtonUp = false;

            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            var container = IsShiftKeyDown ? ParentTreeView.SecondaryContainer ?? this : this;

            switch (e.Key)
            {
                case Key.Up:
                    container.GetPrevious()?.KeyboardSelect();
                    e.Handled = true;
                    break;

                case Key.Down:
                    container.GetNext()?.KeyboardSelect();
                    e.Handled = true;
                    break;

                case Key.Left:
                case Key.Subtract:
                    if (HasItems && IsExpanded)
                    {
                        IsExpanded = false;
                    }
                    else if (ParentItemsControl is MultiSelectTreeViewItem parent)
                    {
                        parent.KeyboardSelect();
                    }
                    e.Handled = true;
                    break;

                case Key.Right:
                case Key.Add:
                    if (HasItems && !IsExpanded)
                    {
                        IsExpanded = true;
                    }
                    e.Handled = true;
                    break;

                case Key.PageUp:
                case Key.Home:
                    ParentTreeView.EnumerateContainers(i => i.IsExpanded).First()?.KeyboardSelect();
                    e.Handled = true;
                    break;

                case Key.PageDown:
                case Key.End:
                    ParentTreeView.EnumerateContainers(i => i.IsExpanded).Last()?.KeyboardSelect();
                    e.Handled = true;
                    break;

                case Key.Space:
                    ToggleSelect();
                    e.Handled = true;
                    break;
            }

            base.OnKeyDown(e);
        }
    }
}
