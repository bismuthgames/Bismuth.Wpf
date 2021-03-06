﻿using System;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bismuth.Wpf.Controls
{
    public class OldMultiSelectTreeViewItem : TreeViewItem
    {
        private static readonly MethodInfo _selectMethod =
            typeof(TreeViewItem).GetMethod("Select", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly object[] _trueParameter = new object[] { true };

        internal void MakePrimary()
        {
            if (!IsSelected) throw new InvalidOperationException("Only selected items can be made primary.");
            // We don't want to trigger selected events when making an item the
            // primary selected item, since it is already selected.
            // To avoid this, we call the private 'Select' method on the base class.
            _selectMethod.Invoke(this, _trueParameter);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new OldMultiSelectTreeViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is OldMultiSelectTreeViewItem;
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            ParentTreeView?.RefreshSelectedItems();
            base.OnItemsChanged(e);
        }

        protected override void OnSelected(RoutedEventArgs e)
        {
            ParentTreeView.AddToSelected(ItemForContainer);
            base.OnSelected(e);
        }

        protected override void OnUnselected(RoutedEventArgs e)
        {
            var parentTreeView = ParentTreeView;
            if (parentTreeView == null) return;

            parentTreeView.RemoveFromSelected(ItemForContainer);

            if (parentTreeView.SecondarySelectedContainer == this)
                parentTreeView.SecondarySelectedContainer = parentTreeView.PrimarySelectedContainer;

            base.OnUnselected(e);
        }

        protected override void OnCollapsed(RoutedEventArgs e)
        {
            UnselectRecursive();
            base.OnCollapsed(e);
        }

        internal void UnselectRecursive()
        {
            for (int i = 0; i < ItemContainerGenerator.Items.Count; i++)
            {
                if (ItemContainerGenerator.ContainerFromIndex(i) is OldMultiSelectTreeViewItem treeViewItem)
                {
                    treeViewItem.IsSelected = false;
                    treeViewItem.UnselectRecursive();
                }
            }
        }

        private static bool IsControlKeyDown => (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;
        private static bool IsShiftKeyDown => (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;

        private bool _suppressMultiSelect;

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (IsShiftKeyDown)
            {
                e.Handled = true;

                if (Keyboard.IsKeyDown(Key.Up))
                {
                    var previous = ParentTreeView.SecondarySelectedContainer.GetPrevious();
                    if (previous != null) ParentTreeView.MultiSelectRange(previous);
                }
                else if (Keyboard.IsKeyDown(Key.Down))
                {
                    var next = ParentTreeView.SecondarySelectedContainer.GetNext();
                    if (next != null) ParentTreeView.MultiSelectRange(next);
                }
                else if (Keyboard.IsKeyDown(Key.PageUp) || Keyboard.IsKeyDown(Key.Home))
                {
                    ParentTreeView.MultiSelectRange(ParentTreeView.EnumerateTreeViewItems(i => i.IsExpanded).First());
                }
                else if (Keyboard.IsKeyDown(Key.PageDown) || Keyboard.IsKeyDown(Key.End))
                {
                    ParentTreeView.MultiSelectRange(ParentTreeView.EnumerateTreeViewItems(i => i.IsExpanded).Last());
                }
            }
            else if (Keyboard.IsKeyDown(Key.Up) ||
                     Keyboard.IsKeyDown(Key.Down) ||
                     Keyboard.IsKeyDown(Key.Left) ||
                     Keyboard.IsKeyDown(Key.Right))
            {
                ParentTreeView.UnselectAllExceptPrimary();
            }

            base.OnKeyDown(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (!e.Handled && IsEnabled && IsSelected)
            {
                if (!IsControlKeyDown && !IsShiftKeyDown)
                {
                    // When "normal" clicking on an already selected item, we want to make it
                    // the primary selected item, and unselect everything else.
                    MakePrimary();
                    ParentTreeView.UnselectAllExceptPrimary();

                    e.Handled = true;
                }
                else if (IsControlKeyDown)
                {
                    if (!_suppressMultiSelect)
                        ParentTreeView.MultiSelect(this);

                    e.Handled = true;
                }
            }

            _suppressMultiSelect = false;

            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (!e.Handled && IsEnabled)
            {
                var parentTreeView = ParentTreeView;

                if (IsControlKeyDown)
                {
                    if (!IsSelected)
                    {
                        _suppressMultiSelect = true;
                        parentTreeView.MultiSelect(this);
                    }

                    e.Handled = true;
                }
                else if (IsShiftKeyDown)
                {
                    parentTreeView.MultiSelectRange(this);

                    e.Handled = true;
                }
                else if (IsSelected)
                {
                    // We don't want the original behavior when clicking on an already selected item,
                    // because we want the unselect to happen at the 'MouseLeftButtonUp' event instead.
                    // If we don't do this, it is not possible to implement drag-drop behavior.
                    e.Handled = true;
                }
                else
                {
                    parentTreeView.UnselectAllExceptPrimary();
                }

                if (e.ClickCount % 2 == 0)
                {
                    IsExpanded = !IsExpanded;
                    e.Handled = true;
                }
            }

            base.OnMouseLeftButtonDown(e);
        }

        internal object ItemForContainer { get { return ParentItemsControl.ItemContainerGenerator.ItemFromContainer(this); } }

        internal ItemsControl ParentItemsControl
        {
            get { return ItemsControlFromItemContainer(this); }
        }

        internal OldMultiSelectTreeView ParentTreeView
        {
            get
            {
                var itemsControl = ParentItemsControl;
                while (itemsControl != null)
                {
                    if (itemsControl is OldMultiSelectTreeView treeView) return treeView;

                    itemsControl = ItemsControlFromItemContainer(itemsControl);
                }

                return null;
            }
        }

        private OldMultiSelectTreeViewItem GetPrevious()
        {
            return ParentTreeView
                .EnumerateTreeViewItems(i => i.IsExpanded)
                .TakeWhile(i => i != this)
                .LastOrDefault();
        }

        private OldMultiSelectTreeViewItem GetNext()
        {
            return ParentTreeView
                .EnumerateTreeViewItems(i => i.IsExpanded)
                .SkipWhile(i => i != this)
                .Skip(1)
                .FirstOrDefault();
        }
    }
}
